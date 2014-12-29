using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security;
using System.ServiceProcess;

namespace Alnet.Common
{
   public abstract class Bootstrapper : Installer
   {
      #region Constants

      /// <summary>
      /// Command-line parameter for username.
      /// </summary>
      private const string PARAMETER_USERNAME = "username";

      /// <summary>
      /// Command-line parameter for password.
      /// </summary>
      private const string PARAMETER_PASSWORD = "password";

      #endregion Constants

      #region Private Fields

      /// <summary>
      ///   Proxy for bootstrapper.
      /// </summary>
      private readonly ServiceProxy _serviceProxy;

      /// <summary>
      ///   To install executable.
      /// </summary>
      private readonly ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();

      /// <summary>
      ///   To install classes that extend ServiceBase.
      /// </summary>
      private readonly ServiceInstaller _serviceInstaller = new ServiceInstaller();

      #endregion

      #region Abstract Members

      /// <summary>
      ///   Starts bootstrapper as service.
      /// </summary>
      protected abstract void OnStart();

      /// <summary>
      ///   Stops bootstrapper as service.
      /// </summary>
      protected abstract void OnStop();

      /// <summary>
      /// Service name used for Service Control Manager.
      /// </summary>
      protected abstract string Name { get; }

      /// <summary>
      /// Service name used in Management Console to display this name for user.
      /// </summary>
      protected abstract string DisplayName { get; }

      /// <summary>
      /// Service description used in Management Console for description column.
      /// </summary>
      protected abstract string Description { get; }

      #endregion

      #region Constructor

      /// <summary>
      ///   Constructor for <see cref="Bootstrapper"/> instance.
      /// </summary>
      protected Bootstrapper()
      {
         // Service Information
         _serviceInstaller.DisplayName = DisplayName;
         _serviceInstaller.StartType = ServiceStartMode.Automatic;
         _serviceInstaller.ServiceName = Name;
         _serviceInstaller.DelayedAutoStart = true;
         _serviceInstaller.Description = Description;
         _serviceInstaller.Committed += serviceInstallerCommitted;

         Installers.Add(_serviceProcessInstaller);
         Installers.Add(_serviceInstaller);

         _serviceProxy = new ServiceProxy(this);
      }

      #endregion

      #region Public Methods

      public static void Run<TBootstrapper>(string[] args)
         where TBootstrapper : Bootstrapper, new()
      {
         var bootstrapper = new TBootstrapper();
         if (args.Length == 0)
         {
            if (Environment.UserInteractive)
            {
               writeInfo(bootstrapper);
               Console.WriteLine("Press any key to exit..");
               Console.ReadKey();
            }
            else
            {
               System.ServiceProcess.ServiceBase.Run(bootstrapper._serviceProxy);
            }
         }
         else
         {
            switch (args[0])
            {
               case "/run":
                  // If windows service is installed we should start its.
                  ServiceController serviceController;
                  try
                  {
                     serviceController = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == bootstrapper.Name);
                  }
                  catch (Win32Exception exception)
                  {
                     var exceptionSummary = string.Format("An exception occured during a getting list of windows services.\r\nException: {0}.\r\n", exception);
                     Console.WriteLine(exceptionSummary);
                     break;
                  }
                  if (serviceController != null)
                  {
                     var status = serviceController.Status;
                     if (status != ServiceControllerStatus.Stopped)
                     {
                        Console.WriteLine("To start service its status should be \"Stopped\".\r\nNow status is {0}.", status);
                        break;
                     }

                     // Check startup type.
                     StartupType? startupType = null;
                     try
                     {
                        startupType = getServiceStartupType(serviceController.ServiceName);
                     }
                     catch (SecurityException exception)
                     {
                        Console.WriteLine("Exception occured during a getting service startup type.\r\n Exception: {0}", exception);
                     }
                     if (!startupType.HasValue)
                     {
                        Console.WriteLine("Couldn't get service startup type.\r\nSo, if service wasn't run check startup type for service \"{0}\" manually.", serviceController.DisplayName);
                     }
                     else if (startupType == StartupType.Disabled)
                     {
                        Console.WriteLine("Service \"{0}\" is disabled. Enable this service and try again.", serviceController.DisplayName);
                        break;
                     }

                     Console.WriteLine("Starting windows service ...");
                     runServiceAction(bootstrapper, ServiceAction.Start);
                     Console.WriteLine("Windows service is started.");
                     break;
                  }

                  // Start service within a console application.
                  Console.WriteLine("Starting Service...");
                  bootstrapper.OnStart();
                  Console.WriteLine("Service is started. Hit any key to stop.{0}", Environment.NewLine);
                  Console.ReadKey(true);
                  Console.WriteLine("Stopping Service...");
                  bootstrapper.OnStop();
                  Console.WriteLine("Exiting.");
                  break;
               case "/install":
                  var runService = (args.Length >= 2) && (args[1] == "/run");

                  // To pass all other arguments to installer.
                  var startIndex = runService ? 2 : 1;
                  var newLength = args.Length - startIndex;
                  var commandLine = new string[newLength];
                  Array.Copy(args, startIndex, commandLine, 0, newLength);

                  try
                  {
                     installService(bootstrapper, runService, commandLine);
                  }
                  catch (Exception exception)
                  {
                     var exceptionSummary = string.Format("An exception occured during installation of the service.\r\nException: {0}.\r\n", exception);
                     Console.WriteLine(exceptionSummary);
                  }

                  break;
               case "/uninstall":
                  uninstallService(bootstrapper);
                  break;
               default:
                  writeInfo(bootstrapper);
                  break;
            }
         }
      }

      #endregion

      #region Protected Methods

      /// <summary>
      /// To set service account if credentials was passed via command-line.
      /// </summary>
      /// <param name="savedState">Contains the state of the computer before the installers in the Installers property are installed.</param>
      protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
      {
         base.OnBeforeInstall(savedState);

         if (String.IsNullOrWhiteSpace(Context.Parameters.ContainsKey(PARAMETER_USERNAME) ? Context.Parameters[PARAMETER_USERNAME] : null) ||
             String.IsNullOrWhiteSpace(Context.Parameters.ContainsKey(PARAMETER_PASSWORD) ? Context.Parameters[PARAMETER_PASSWORD] : null))
         {
            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
         }
         else
         {
            _serviceProcessInstaller.Account = ServiceAccount.User;
            _serviceProcessInstaller.Username = Context.Parameters[PARAMETER_USERNAME].Trim();
            _serviceProcessInstaller.Password = Context.Parameters[PARAMETER_PASSWORD].Trim();
         }
      }

      #endregion Protected Methods

      #region Private Members

      /// <summary>
      /// Returns windows service startup type.
      /// </summary>
      /// <param name="serviceName">Service name.</param>
      /// <returns>Startup type.</returns>
      /// <exception cref="SecurityException"></exception>
      private static StartupType? getServiceStartupType(string serviceName)
      {
         StartupType? state = null;

         string wmiQuery = @"SELECT * FROM Win32_Service WHERE Name='" + serviceName + @"'";
         ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
         ManagementObjectCollection services = searcher.Get();
         if (services.Count == 1)
         {
            foreach (ManagementObject service in services)
            {
               StartupType parsedValue;
               if (Enum.TryParse(service["StartMode"].ToString(), out parsedValue))
               {
                  state = parsedValue;
               }
            }
         }
         return state;
      }

      /// <summary>
      ///   Install assembly as windows service.
      /// </summary>
      /// <param name="description">Service description.</param>
      /// <param name="isRunService">Windows service will be running after installation.</param>
      /// <param name="commandLine">Command line.</param>
      private static void installService(Bootstrapper description, bool isRunService, string[] commandLine)
      {
         var installer = getInstaller(commandLine);

         installer.Install(null);
         installer.Commit(null);

         if (isRunService)
         {
            runServiceAction(description, ServiceAction.Start);
         }
      }

      /// <summary>
      ///   Uninstall assembly as windows service.
      /// </summary>
      private static void uninstallService(Bootstrapper description)
      {
         runServiceAction(description, ServiceAction.Stop);

         getInstaller().Uninstall(null);
      }

      /// <summary>
      /// Handles <see cref="Installer.Committed"/> from service installer.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void serviceInstallerCommitted(object sender, InstallEventArgs e)
      {
         setRecoveryOptions();
      }

      /// <summary>
      /// Sets the recover options to infinit automatic restart each 1 minute on failures.
      /// </summary>
      private void setRecoveryOptions()
      {
         using (var process = new Process())
         {
            var startInfo = process.StartInfo;
            startInfo.FileName = "sc";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // tell Windows that the service should restart if it fails after 1 minute
            startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", Name);

            process.Start();
            process.WaitForExit();

            process.Close();
         }
      }

      /// <summary>
      /// Gets installer for entry point assembly.
      /// </summary>
      /// <param name="commandLine">Command line.</param>
      /// <returns></returns>
      private static Installer getInstaller(string[] commandLine = null)
      {
         var assembly = Assembly.GetEntryAssembly();
         return new AssemblyInstaller(assembly, commandLine) { UseNewContext = true };
      }

      /// <summary>
      ///   Runs action on the service by name.
      /// </summary>
      /// <param name="description">Service description.</param>
      /// <param name="action">Windows service action of type <see cref="ServiceAction"/>.</param>
      private static void runServiceAction(Bootstrapper description, ServiceAction action)
      {
         var serviceController = new ServiceController(description.Name);
         var actionName = string.Empty;

         try
         {
            switch (action)
            {
               case ServiceAction.Start:
                  serviceController.Start();
                  actionName = "start";
                  break;
               case ServiceAction.Stop:
                  serviceController.Stop();
                  actionName = "stop";
                  break;
            }
         }
         catch (Exception ex)
         {
            var error = string.Format(@"The service could not be {0}ed. Please {0} the service manually. Error: {1}", actionName, ex.Message);
         }
      }

      /// <summary>
      ///   Writes program info.
      /// </summary>
      /// <param name="description">Service description.</param>
      private static void writeInfo(Bootstrapper description)
      {
         var foreground = Console.ForegroundColor;
         Console.ForegroundColor = ConsoleColor.Cyan;

         var pathToExe = Environment.GetCommandLineArgs()[0];
         var fileName = Path.GetFileName(pathToExe);

         Console.WriteLine(description.Description);
         Console.WriteLine("(c) {0} ELVEES NeoTek CJSC. All rights reserved.", DateTime.Now.Year);

         Console.WriteLine("Parameters:");
         Console.WriteLine(@"/run: - runs application as console application.");
         Console.WriteLine(@"/install [/{0}=DOMAIN\\user /{1}=userpwd]: - installs application as windows service.", PARAMETER_USERNAME, PARAMETER_PASSWORD);
         Console.WriteLine(@"/install /run [/{0}=DOMAIN\\user /{1}=userpwd]: - the same as /install but also starts windows service after installation.", PARAMETER_USERNAME, PARAMETER_PASSWORD);
         Console.WriteLine(@"/uninstall: - uninstalls application as windows service.");
         Console.WriteLine();

         Console.WriteLine("Examples:");
         Console.WriteLine("{0} /run", fileName);
         Console.WriteLine("{0} /install", fileName);
         Console.WriteLine("{0} /install /{1}=DOMAIN\\user /{2}=userpwd", fileName, PARAMETER_USERNAME, PARAMETER_PASSWORD);
         Console.WriteLine("{0} /install /run", fileName);
         Console.WriteLine("{0} /install /run /{1}=DOMAIN\\user /{2}=userpwd", fileName, PARAMETER_USERNAME, PARAMETER_PASSWORD);
         Console.WriteLine("{0} /uninstall", fileName);
         Console.WriteLine();

         Console.ForegroundColor = foreground;
      }

      #endregion

      #region Nested Type: StartupType

      /// <summary>
      /// Windows service startup type.
      /// </summary>
      private enum StartupType
      {
         Auto,

         Manual,

         Disabled
      }

      #endregion

      #region Nested Type: Service Action

      /// <summary>
      ///   Windows service actions.
      /// </summary>
      private enum ServiceAction
      {
         /// <summary>
         ///   Start windows service.
         /// </summary>
         Start,

         /// <summary>
         ///   Stop windows service
         /// </summary>
         Stop
      }

      #endregion

      #region Nested Type: ServiceProxy

      /// <summary>
      ///   Service proxy for bootstrapper.
      /// </summary>
      private sealed class ServiceProxy : System.ServiceProcess.ServiceBase
      {
         #region Private Fields

         /// <summary>
         ///   Bootstrapper as target instance.
         /// </summary>
         private readonly Bootstrapper _target;

         #endregion

         #region Constructor

         public ServiceProxy(Bootstrapper target)
         {
            _target = target;
         }

         #endregion

         #region Overrides of ServiceBase

         protected override void OnStart(string[] args)
         {
            try
            {
               base.OnStart(args);
               _target.OnStart();
            }
            catch (Exception ex)
            {
               throw;
            }
         }

         protected override void OnStop()
         {
            try
            {
               _target.OnStop();
               base.OnStop();
            }
            catch (Exception ex)
            {
               throw;
            }
         }

         #endregion
      }

      #endregion
   }
}

using Alnet.Common;

namespace Alnet.AudioServer
{
   class Program
   {
      static void Main(string[] args)
      {
         Bootstrapper.Run<AppBootstrapper>(args);
      }
   }
}

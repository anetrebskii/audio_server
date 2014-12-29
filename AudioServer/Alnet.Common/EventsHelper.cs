using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   /// Utility class that provides automated asynchronous event publishing.  This is based on Example 7-14
   /// in Juval Lowy's "Programming .NET Components" (pages 138-139).
   /// </summary>
   public static class EventsHelper
   {
      /// <summary>
      /// Ensure that an event handler can only be registered once for a single event.
      /// </summary>
      /// <param name="del">The event delegate that is being registered to</param>
      /// <param name="value">The event handler that is being registered</param>
      public static Delegate AddUniqueSubscriber(Delegate del, Delegate value)
      {
         if (del != null)
         {
            // Check all existing registered delegate handlers and if the new handler
            // is already registered, then simply exit.
            if (del.GetInvocationList().Contains(value))
            {
               return del;
            }
         }

         // Register the new event handler with the event delegate
         return Delegate.Combine(del, value);
      }

      /// <summary>
      /// Returns the invocation list of the delegate.
      /// </summary>
      /// <param name="delegate">Delegate.</param>
      /// <returns>Invocation list.</returns>
      public static Delegate[] GetInvocationList([CanBeNull]Delegate @delegate)
      {
         if (@delegate == null)
         {
            return new Delegate[0];
         }

         return @delegate.GetInvocationList();
      }


      /// <summary>
      /// Execute command if it can execute with such argument.
      /// </summary>
      /// <param name="command">Command.</param>
      /// <param name="argument">Argument of command.</param>
      /// <returns>Return true if command was executed.</returns>
      public static bool ExecuteIfCan(this ICommand command, object argument)
      {
         if (command != null && command.CanExecute(argument))
         {
            command.Execute(argument);
            return true;
         }
         return false;
      }

      /// <summary>
      /// Executes del with args on the calling thread, ensuring that each callback in delegates invocation list
      /// is called, even if one or more raise an exception.
      /// </summary>
      [DebuggerStepThrough]
      public static void SafeInvoke(Delegate @delegate, params object[] args)
      {
         if (@delegate == null)
         {
            return;
         }

         // any exceptions that were caught during notification
         Exception exception = null;

         // get the list of delegates that need to be called
         var delegates = @delegate.GetInvocationList();
         foreach (var sink in delegates)
         {
            try
            {
               sink.DynamicInvoke(args);
            }
            catch (Exception e)
            {
               // only save off the first exception that we catch, to throw later
               if (null == exception)
               {
                  exception = e;
               }
            }
         }

         // only throw the exception after we have notified all parties
         if (null != exception)
            throw exception;
      }

      /// <summary>
      /// Invokes event in safe manner.
      /// </summary>
      /// <param name="handler">Handler to be called.</param>
      /// <param name="sender">Handler's "sender" argument.</param>
      /// <param name="args">Handler's "args" argument.</param>
      [DebuggerStepThrough]
      public static void InvokeEventHandler<T>(EventHandler<T> handler, object sender, T args)
          where T : EventArgs
      {
         var h = handler;
         if (h != null)
         {
            h(sender, args);
         }
      }

      /// <summary>
      /// Invokes event in safe manner.
      /// </summary>
      /// <param name="handler">Handler to be called.</param>
      /// <param name="sender">Handler's "sender" argument.</param>
      /// <param name="args">Event data.</param>
      [DebuggerStepThrough]
      public static void InvokeEventHandler<T>(EventHandler<DataEventArgs<T>> handler, object sender, T args)
      {
         var h = handler;
         if (h != null)
         {
            h(sender, new DataEventArgs<T>(args));
         }
      }

      /// <summary>
      /// Invokes event in safe manner.
      /// </summary>
      /// <param name="handler">Handler to be called.</param>
      /// <param name="sender">Handler's "sender" argument.</param>
      /// <param name="args">Handler's "args" argument.</param>
      [DebuggerStepThrough]
      public static void InvokeEventHandler(EventHandler handler, object sender, EventArgs args)
      {
         var h = handler;
         if (h != null)
         {
            h(sender, args);
         }
      }

      /// <summary>
      /// Invokes delegate in safe manner.
      /// </summary>
      /// <param name="delegate">Delegate to be called.</param>
      /// <param name="args">Delegate's Invoke arguments.</param>
      [DebuggerStepThrough]
      public static void InvokeEventHandler(Delegate @delegate, params object[] args)
      {
         var reference = @delegate;
         if (reference != null)
         {
            reference.DynamicInvoke(args);
         }
      }
   }
}

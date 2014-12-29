using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCTV.Framework.Utility;

namespace Alnet.Common.Collections
{
   /// <summary>
   /// Represents a collection of weak events.
   /// </summary>
   /// <remarks>
   /// This class is thread safe.
   /// </remarks>
   /// <typeparam name="TEventArgs">Weak event argument type.</typeparam>
   public sealed class WeakEventCollection<TEventArgs> :IDisposable
        where TEventArgs : EventArgs
   {
      #region Private Properties

      /// <summary>
      /// List of weak reference to target of invocation, and <see cref="MethodInfo"/> of event targets.
      /// </summary>
      private readonly IList<Tuple<WeakReference, MethodInfo>> _subscribersList = new List<Tuple<WeakReference, MethodInfo>>();

      /// <summary>
      /// To lock shared resources among threads.
      /// </summary>
      private readonly object _lockObject = new object();

      /// <summary>
      /// Dispose guard.
      /// </summary>
      private readonly DisposedGuard _disposedGuard = new DisposedGuard("WeakEventCollection");

      #endregion

      #region Public Members

      /// <summary>
      /// Count of subscribers.
      /// </summary>
      public int SubscribersCount
      {
         get
         {
            lock(_lockObject)
            {
               return _subscribersList.Count;
            }
         }
      }

      /// <summary>
      /// Adds subscriber to weak event collection.
      /// </summary>
      /// <param name="subscriber"></param>
      public void AddSubscriber(EventHandler<TEventArgs> subscriber)
      {
         _disposedGuard.Check();
         Guard.VerifyArgumentNotNull(subscriber, "subscriber");

         lock (_lockObject)
         {
            if (_subscribersList.Where(tuple => tuple.Item1.IsAlive && tuple.Item2 == subscriber.Method).Any(weakReference => Equals(weakReference.Item1.Target, subscriber)))
            {
               return;
            }
            _subscribersList.Add(new Tuple<WeakReference, MethodInfo>(new WeakReference(subscriber.Target), subscriber.Method));
         }
      }

      /// <summary>
      /// Removes subscriber from weak event collection.
      /// </summary>
      /// <param name="subscriber"></param>
      public void RemoveSubscriber(EventHandler<TEventArgs> subscriber)
      {
         _disposedGuard.Check();
         Guard.VerifyArgumentNotNull(subscriber, "subscriber");
         lock (_lockObject)
         {
            var item = _subscribersList.FirstOrDefault(tuple => tuple.Item1.Target == subscriber.Target && tuple.Item2 == subscriber.Method);
            if (item != null)
            {
               _subscribersList.Remove(item);
            }
         }
      }

      /// <summary>
      /// Fires event.
      /// </summary>
      /// <param name="sender">Event sender.</param>
      /// <param name="arg">Event arguments.</param>
      public void Fire(object sender, TEventArgs arg)
      {
         fire(false, sender, arg);
      }

      /// <summary>
      /// Fires event.
      /// </summary>
      /// <remarks>
      /// Prevent all non-critical exceptions to be risen from targets of invocation.
      /// </remarks>
      /// <param name="sender">Event sender.</param>
      /// <param name="arg">Event arguments.</param>
      /// <exception cref="AggregateException">If one or more event handlers throws an exception.</exception>
      public void SafeFire(object sender, TEventArgs arg)
      {
         fire(true, sender, arg);
      }

      #endregion

      #region Implementation of IDisposable

      /// <summary>
      /// Implements <see cref="IDisposable.Dispose"/>.
      /// </summary>
      public void Dispose()
      {
         _disposedGuard.Dispose();

         lock(_lockObject)
         {
            _subscribersList.Clear();
         }
      }

      #endregion

      #region Private members

      /// <summary>
      /// Fires event.
      /// </summary>
      /// <param name="sender">Event sender.</param>
      /// <param name="arg">Event arguments.</param>
      /// <param name="safe">Prevent non-critical exception to be raised.</param>
      private void fire(bool safe, object sender, TEventArgs arg)
      {
         _disposedGuard.Check();

         var exceptions = new List<Exception>();

         var parameters = new[] {sender, arg};

         var toRemove = new List<Tuple<WeakReference, MethodInfo>>();
         Tuple<WeakReference, MethodInfo>[] subsribers = null;
         lock(_lockObject)
         {
            // Copying collection is using for case, if user will unsubscribe from the event in the event handle.
            subsribers = _subscribersList.ToArray();
         }

         foreach(var tuple in subsribers)
         {
            if(!tuple.Item1.IsAlive)
            {
               toRemove.Add(tuple);
               continue;
            }

            var target = tuple.Item1.Target;

            if(target == null)
            {
               toRemove.Add(tuple);
               continue;
            }

            if(!safe)
            {
               tuple.Item2.Invoke(target, parameters);
            }
            else
            {
               try
               {
                  tuple.Item2.Invoke(target, parameters);
               }
               catch(Exception e)
               {
                  Guard.RethrowIfFatal(e);
                  exceptions.Add(e);
               }
            }
         }

         lock(_lockObject)
         {
            foreach(var tuple in toRemove)
            {
               if(_subscribersList.Contains(tuple))
               {
                  _subscribersList.Remove(tuple);
               }
            }
         }

         if(exceptions.Any())
         {
            throw new AggregateException("One or more target invocation exception occurred", exceptions);
         }
      }

      #endregion
   }
}

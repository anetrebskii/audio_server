#region Copyright

// (c) 2014 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   /// Helper for working with list of <see cref="EventWaitHandle"/>. Setting, waiting, executing some action on some event.
   /// </summary>
   public sealed class WaitEventsHelper : IDisposable
   {
      #region Constants

      /// <summary>
      /// Identificator for AllUsers group in NT systems.
      /// </summary>
      private const string ALL_USERS_SECURITY_IDENTIFICATOR = "S-1-1-0";

      #endregion

      #region Fields

      /// <summary>
      /// Instance of <see cref="DisposedGuard"/>.
      /// </summary>
      [NotNull]
      private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(WaitEventsHelper));

      /// <summary>
      /// Indicator that <see cref="StartEventsListening"/> is in process. 
      /// </summary>
      private bool _isStarted;

      /// <summary>
      /// Relations between created wait handler's and actions that should be executed on it setting.
      /// </summary>
      private readonly Dictionary<EventWaitHandle, Action> _actionsToListen = new Dictionary<EventWaitHandle, Action>();

      /// <summary>
      /// Relations between wait handler name and it's instance.
      /// </summary>
      private readonly Dictionary<string, EventWaitHandle> _waitHandles = new Dictionary<string, EventWaitHandle>();

      /// <summary>
      /// Event for stopping wait handler's listening.
      /// </summary>
      [CanBeNull]
      private ManualResetEvent _eventForExitListenThread;

      /// <summary>
      /// Thread in wich events listening is occured.
      /// </summary>
      [CanBeNull]
      private Thread _eventsListnerThread;

      #endregion

      #region Public methods

      /// <summary>
      /// Call <see cref="EventWaitHandle.Set"/> for <see cref="eventName"/> event.
      /// </summary>
      /// <param name="eventName">Event name.</param>
      public void SetEvent(string eventName)
      {
         _disposedGuard.Check();
         Guard.VerifyArgumentNotNullOrWhiteSpace(eventName, "eventName");

         EventWaitHandle waitHandle;
         if(_waitHandles.TryGetValue(eventName, out waitHandle))
         {
            waitHandle.Set();
         }
         else
         {
            throw new ApplicationException("Event with given name is not exists");
         }
      }

      /// <summary>
      /// Remove event from created list.
      /// </summary>
      /// <param name="eventName">Event name.</param>
      public void RemoveEvent([NotNull]string eventName)
      {
         _disposedGuard.Check();
         if(_isStarted)
         {
            throw new ApplicationException("Events listner thread is already started.");
         }

         Guard.VerifyArgumentNotNullOrWhiteSpace(eventName, "eventName");

         removeEvent(eventName);
      }

      /// <summary>
      /// Clear all created events.
      /// </summary>
      public void ClearEvents()
      {
         _disposedGuard.Check();
         if(_isStarted)
         {
            throw new ApplicationException("Events listner thread is already started.");
         }

         clearEvents();
      }

      /// <summary>
      /// Create new <see cref="EventWaitHandle"/>. Store it locally.
      /// </summary>
      /// <param name="eventName">Event name.</param>
      /// <param name="executeAction">Action that should be executed when event is setted.</param>
      /// <returns>If created new <see cref="EventWaitHandle"/> then returns true. Otherwise - false.</returns>
      public bool AddEvent(string eventName, [CanBeNull] Action executeAction = null)
      {
         _disposedGuard.Check();
         Guard.VerifyArgumentNotNullOrWhiteSpace(eventName, "eventName");
         if(_isStarted)
         {
            throw new ApplicationException("Events listner thread is already started.");
         }

         if(_waitHandles.ContainsKey(eventName))
         {
            throw new ApplicationException("Event with given name is already exists");
         }

         bool eventWaitHandleCreatedNew = true;
         EventWaitHandle eventWaitHandle;

         if(EventWaitHandle.TryOpenExisting(eventName, EventWaitHandleRights.FullControl, out eventWaitHandle))
         {
            eventWaitHandleCreatedNew = false;
         }
         else
         {
            // AllUsers in NT systems.
            SecurityIdentifier networkService = new SecurityIdentifier(ALL_USERS_SECURITY_IDENTIFICATOR);
            IdentityReference networkServiceIdentity = networkService.Translate(typeof(NTAccount));

            var securityInfo = new EventWaitHandleSecurity();
            securityInfo.SetAccessRule(new EventWaitHandleAccessRule(networkServiceIdentity, EventWaitHandleRights.FullControl, AccessControlType.Allow));

            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName, out eventWaitHandleCreatedNew, securityInfo);
         }

         _actionsToListen.Add(eventWaitHandle, executeAction);
         _waitHandles.Add(eventName, eventWaitHandle);

         return eventWaitHandleCreatedNew;
      }

      /// <summary>
      /// Start listening calling events added throw <see cref="AddEvent"/>.
      /// </summary>
      public void StartEventsListening()
      {
         _disposedGuard.Check();

         if(_isStarted)
         {
            throw new ApplicationException("Events listner thread is already started.");
         }

         Guard.VerifyArgument(_waitHandles.Count != 0, "List of event wait handles is empty");

         _isStarted = true;

         _eventForExitListenThread = new ManualResetEvent(false);

         _eventsListnerThread = new Thread(waitEvents);
         _eventsListnerThread.Start();

         // Spin for a while waiting for the started thread to become alive:
         while(!_eventsListnerThread.IsAlive)
         {
            Thread.Sleep(0);
         }
      }

      /// <summary>
      /// Stop listening calling events added throw <see cref="AddEvent"/>.
      /// </summary>
      public void StopEventsListening()
      {
         _disposedGuard.Check();

         if(!_isStarted)
         {
            throw new ApplicationException("Events listner thread is already started.");
         }

         stopEventsListening();
      }

      #endregion

      #region Implementation of IDisposable

      /// <summary>
      /// Implementation of <see cref="IDisposable.Dispose"/>.
      /// </summary>
      void IDisposable.Dispose()
      {
         _disposedGuard.Dispose();

         if(_isStarted)
         {
            stopEventsListening();
         }

         clearEvents();
      }

      #endregion

      #region Private methods

      /// <summary>
      /// Clear locally created events.
      /// </summary>
      private void clearEvents()
      {
         foreach(var item in _waitHandles.Keys.ToArray())
         {
            removeEvent(item);
         }
      }

      /// <summary>
      /// Remove created event from local storage.
      /// </summary>
      /// <param name="eventName">Event name.</param>
      /// <exception cref="ApplicationException">If event is not exists in <see cref="_waitHandles"/>.</exception>
      private void removeEvent(string eventName)
      {
         EventWaitHandle waitHandle;
         if(_waitHandles.TryGetValue(eventName, out waitHandle))
         {
            _waitHandles.Remove(eventName);
            _actionsToListen.Remove(waitHandle);

            waitHandle.SafeDispose();
         }
         else
         {
            throw new ApplicationException("Event with given name is not exists");
         }
      }

      /// <summary>
      /// Stop listening events stored in <see cref="_waitHandles"/>.
      /// </summary>
      private void stopEventsListening()
      {
         _eventForExitListenThread.Set();
         _eventsListnerThread.Join();

         _eventForExitListenThread.Dispose();

         _eventForExitListenThread = null;
         _eventsListnerThread = null;

         _isStarted = false;
      }

      /// <summary>
      /// Start listening events stored in <see cref="_waitHandles"/>.
      /// </summary>
      private void waitEvents()
      {
         if(_disposedGuard.IsDisposed)
         {
            return;
         }

         var eventHandlers = new List<EventWaitHandle>() {_eventForExitListenThread};
         eventHandlers.AddRange(_waitHandles.Values);

         while(true)
         {
            var index = WaitHandle.WaitAny(eventHandlers.ToArray());

            if(index == eventHandlers.IndexOf(_eventForExitListenThread))
            {
               return;
            }

            var executedEvent = eventHandlers[index];

            var executeAction = _actionsToListen[executedEvent];
            if(executeAction != null)
            {
               executeAction();
            }
         }
      }

      #endregion
   }
}
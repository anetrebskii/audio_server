#region Copyright

// © 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Alnet.Common
{
   /// <summary>
   /// Provide execute different operations in one thread of the pool.
   /// </summary>
   public sealed class QueueProcessor : IDisposable
   {
      /// <summary>
      /// Indicating is processing queue.
      /// 1 - is processing.
      /// 0 - is not processing.
      /// </summary>
      private int _isProcessingQueue;

      /// <summary>
      /// The queue of actions.
      /// </summary>
      private ConcurrentQueue<Action<CancellationToken>> _queueActions = new ConcurrentQueue<Action<CancellationToken>>();

      /// <summary>
      /// The disposed guard
      /// </summary>
      private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(QueueProcessor));

      /// <summary>
      /// The synchronization object for process the queue
      /// </summary>
      private readonly object _syncProcessQueue = new object();

      /// <summary>
      /// The cancellation token source.
      /// </summary>
      private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

      /// <summary>
      /// Add operation to queue for execute.
      /// </summary>
      /// <param name="action">The action.</param>
      public void Enqueue(Action action)
      {         
         _disposedGuard.Check();
         _queueActions.Enqueue(token => action());
         processQueue();
      }

      /// <summary>
      /// Add operation to queue for execute.
      /// </summary>
      /// <param name="actionWithCancellation">The action with cancellation.</param>
      public void Enqueue(Action<CancellationToken> actionWithCancellation)
      {
         _disposedGuard.Check();
         _queueActions.Enqueue(actionWithCancellation);
         processQueue();
      }

      /// <summary>
      /// Cancels all operations.
      /// </summary>
      public void Cancel()
      {
         _queueActions = new ConcurrentQueue<Action<CancellationToken>>();
         _cancellationTokenSource.Cancel();
         CancellationTokenSource oldCancellationTokenSource = _cancellationTokenSource;
         _cancellationTokenSource = new CancellationTokenSource();
         oldCancellationTokenSource.Dispose();         
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      void IDisposable.Dispose()
      {
         _cancellationTokenSource.Cancel();
         lock (_syncProcessQueue)
         {
            _disposedGuard.Dispose();            
            _queueActions = new ConcurrentQueue<Action<CancellationToken>>();
         }                 
      }

      /// <summary>
      /// Processes the queue <seealso cref="_queueActions"/>. Provide process queue in a one thread.
      /// </summary>
      private void processQueue()
      {
         if (_disposedGuard.IsDisposed)
         {
            return;
         }
         if (Interlocked.CompareExchange(ref _isProcessingQueue, 1, 0) == 0)
         {
            ThreadPool.QueueUserWorkItem(state =>
            {
               lock (_syncProcessQueue)
               {
                  try
                  {
                     Action<CancellationToken> actionFromQueue;
                     while (_queueActions.TryDequeue(out actionFromQueue) && !_disposedGuard.IsDisposed && 
                        !_cancellationTokenSource.IsCancellationRequested)
                     {
                        actionFromQueue.Invoke(_cancellationTokenSource.Token);
                     }
                  }
                  finally
                  {
                     Interlocked.Exchange(ref _isProcessingQueue, 0);
                  }
                  if (_queueActions.Count > 0)
                  {
                     processQueue();
                  }
               }
            });
         }
      }
   }
}

using System;
using System.Threading;
using CCTV.Framework.Utility;

namespace Alnet.Common
{
   /// <summary>
   /// Implements <see cref="IDisposable"/> and <see langword="using"/> pattern for easy-to-use locking with possibility of cancelling.
   /// </summary>
   /// <example>
   /// <code title="General pattern of usage" description="" lang="CS">
   /// <![CDATA[
   /// CancellableLock _lock = new CancellableLock();
   /// void thread1(CancellationToken token)
   /// {
   ///   using(_lock.Lock(token))
   ///   {
   ///      // Do stuff
   ///   }
   /// }
   /// void thread2()
   /// {
   ///   using(_lock.Lock())
   ///   {
   ///      // Do stuff
   ///   }
   /// }
   /// ]]>
   /// </code>
   /// </example>
   /// <remarks>
   /// This object allows recirsive locks.
   /// <see cref="IDisposable"/> object that returned by <see cref="Lock"/> have no meaning after <see cref="Dispose"/> call.
   /// </remarks>
   public sealed class CancellableLock : IDisposable
   {
      #region Private fields

      /// <summary>
      /// Synchronization object.
      /// </summary>
      private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

      /// <summary>
      /// Synchronization object for <see cref="_lockCount"/> and <see cref="_owningThread"/>.
      /// </summary>
      private readonly object _synckRoot = new object();

      /// <summary>
      /// Indicates which thread currently holds the lock. If <see langword="null"/>, then the lock is available.
      /// </summary>
      private volatile Thread _owningThread;

      /// <summary>
      /// Lock count.
      /// </summary>
      /// <remarks>
      /// Invariant: if <see cref="_lockCount"/> == 0 than <see cref="_owningThread"/> also == null, and <see cref="_semaphore"/> is released.
      /// </remarks>
      private long _lockCount;

      /// <summary>
      /// Dispose Guard.
      /// </summary>
      private readonly DisposedGuard _disposedGuard = DisposedGuard.Create<CancellableLock>();

      #endregion

      #region Public members

      /// <summary>
      /// Locks synchronization object.
      /// </summary>
      /// <param name="token">Cancellation token.</param>
      /// <exception cref="OperationCanceledException">if <paramref name="token"/> requests for cancellation.</exception>
      /// <returns><see cref="IDisposable"/> object that should be disposed to release lock.</returns>
      public IDisposable Lock(CancellationToken token = default(CancellationToken))
      {
         token.ThrowIfCancellationRequested();
         lock (_synckRoot)
         {
            if (_owningThread == Thread.CurrentThread)
            {
               ++_lockCount;
               // Nested wait. pass without lock.
               return new Waiter(this);
            }
         }
         _semaphore.Wait(token);
         lock (_synckRoot)
         {
            Guard.Assert(_owningThread == null, "Lock is still owned.");
            Guard.Assert(_lockCount == 0, "Lock wrong."); // Just Invariant check.
            _owningThread = Thread.CurrentThread; // Remember current thread.
            _lockCount = 1;
            return new Waiter(this);
         }
      }
      
      /// <summary>
      /// Implements <see cref="IDisposable.Dispose"/>.
      /// </summary>
      public void Dispose()
      {
         _disposedGuard.Dispose();
         _semaphore.Dispose();
      }

      #endregion

      #region Private members

      /// <summary>
      /// Unlock called from <see cref="Waiter.Dispose"/>.
      /// </summary>
      private void unlock()
      {
         _disposedGuard.Check();
         lock (_synckRoot)
         {
            Guard.Assert(_lockCount > 0, "Wrong amount of release.");
            --_lockCount;
            if (_lockCount == 0)
            {
               _owningThread = null;
               _semaphore.Release();
            }
         }
      }

      #endregion

      #region Private class : Waiter

      /// <summary>
      /// Internal object that will be returned as <see cref="IDisposable"/> object for release.
      /// </summary>
      private sealed class Waiter : IDisposable
      {
         private readonly DisposedGuard _disposedGuard = DisposedGuard.Create<Waiter>();

         private readonly CancellableLock _host;

         public Waiter(CancellableLock host)
         {
            _host = host;
         }

         public void Dispose()
         {
            _disposedGuard.Dispose();
            _host.unlock();
         }
      }

      #endregion
   }
}

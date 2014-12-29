#region Copyright

// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

#region

using System;
using System.Diagnostics;
using System.Threading;
using Alnet.Common;

#endregion

// ReSharper disable once CheckNamespace
namespace CCTV.Framework.Utility
{
   /// <summary>
   ///   Precise timer realization.
   /// </summary>
   /// <remarks>
   ///   The default timers have an imprecise intervals by calling actions. This timer applies adaptive methodics to handle default timer.
   ///   This timer can be used with up to 15 ms interval ideally (system limit).
   /// </remarks>
   public sealed class PreciseTimer: IDisposable
   {
      #region Private Fields

      /// <summary>
      ///   Imprecise timer to fire callbacks with any time errors.
      /// </summary>
      private readonly Timer _timer;

      /// <summary>
      ///   Represents accurately measured elapsed time.
      /// </summary>
      private readonly Stopwatch _watcher = new Stopwatch();

      /// <summary>
      ///   Ideal frequency of timer.
      /// </summary>
      private readonly int _idealPeriod;

      /// <summary>
      ///   Callback action for precise timer.
      /// </summary>
      private readonly Action _timerCallback;

      /// <summary>
      ///   Counts invokes of the timer callbacks.
      /// </summary>
      private long _timerCounter;

      #endregion

      #region Constructor

      /// <summary>
      ///   Constructor for <see cref="PreciseTimer"/> instance.
      /// </summary>
      /// <param name="callback">Callback action for precise timer.</param>
      /// <param name="period">Period for precise timer. It sholdn't be less than 15 ms. Lower values will be ignored.</param>
      public PreciseTimer(Action callback, int period)
      {
         Guard.VerifyArgument(period > 0, "Period must be great than zero");
         _idealPeriod = period;
         _timerCallback = Guard.EnsureArgumentNotNull(callback, "callback");

         _watcher.Start();
         _timer = new Timer(timerOnElapsed, null, period, period);
      }

      #endregion

      #region Implementation of IDispose

      /// <summary>
      ///   Implements <see cref="IDisposable.Dispose()"/>.
      /// </summary>
      public void Dispose()
      {
         _watcher.Stop();
         _timer.Dispose();
      }

      #endregion

      #region Private Methods

      /// <summary>
      ///   Apply adaptive methodics to the timer.
      /// </summary>
      private void adjustTimer()
      {
         ++_timerCounter;
         long elapsed = _watcher.ElapsedMilliseconds;
         long error = elapsed - _timerCounter * _idealPeriod;

         if (error > 0)
         {
            long correction = _idealPeriod - error % _idealPeriod;
            _timer.Change(correction, 0);
         }
         else
         {
            _timer.Change(_idealPeriod, _idealPeriod);
         }
      }

      /// <summary>
      ///   Adjust timer and fire callback.
      /// </summary>
      private void timerOnElapsed(object state)
      {
         adjustTimer();
         _timerCallback();
      }

      #endregion
   }
}

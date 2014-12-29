using System.Threading;

namespace Alnet.Common
{
   /// <summary>
   /// Thread safe implementation of bool flag.
   /// </summary>
   public sealed class InterlockedBool
   {
      /// <summary>
      /// Value of true state.
      /// </summary>
      private const int TRUE = 1;

      /// <summary>
      /// Value of false state.
      /// </summary>
      private const int FALSE = 0;

      /// <summary>
      /// Current state.
      /// </summary>
      private int _state;

      /// <summary>
      /// Initializes a new instance of <see cref="InterlockedBool"/> class.
      /// </summary>
      /// <param name="initial">Initial value.</param>
      public InterlockedBool(bool initial = false)
      {
         _state = initial ? TRUE : FALSE;
      }

      /// <summary>
      /// Set new value and return old one in thread safe manner.
      /// </summary>
      /// <param name="newValue">New value.</param>
      /// <returns>Old value.</returns>
      public bool Exchange(bool newValue)
      {
         return Interlocked.Exchange(ref _state, newValue ? TRUE : FALSE) == TRUE;
      }
   }
}

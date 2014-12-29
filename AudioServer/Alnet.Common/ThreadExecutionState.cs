#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

#region

using System;

#endregion

namespace Alnet.Common
{
   /// <summary>
   /// Thread power flags.
   /// </summary>
   [Flags]
   public enum ThreadExecutionState : uint
   {
      /// <summary>
      /// Enables away mode. This value must be specified with ES_CONTINUOUS.
      /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping. See Remarks.
      /// Windows Server 2003 and Windows XP:  ES_AWAYMODE_REQUIRED is not supported.
      /// </summary>
      ES_AWAYMODE_REQUIRED = 0x00000040,

      /// <summary>
      /// Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
      /// </summary>
      ES_CONTINUOUS = 0x80000000,

      /// <summary>
      /// Forces the display to be on by resetting the display idle timer.
      /// Windows 8:  This flag can only keep a display turned on, it can't turn on a display that's currently off.
      /// </summary>
      ES_DISPLAY_REQUIRED = 0x00000002,

      /// <summary>
      /// Forces the system to be in the working state by resetting the system idle timer.
      /// </summary>
      ES_SYSTEM_REQUIRED = 0x00000001
   }
}
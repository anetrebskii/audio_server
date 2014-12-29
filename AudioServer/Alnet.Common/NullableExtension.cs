#region Copyright

// (c) 2014 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

namespace CCTV.Framework.Utility
{
   /// <summary>
   /// Extension for printing nullable values. 
   /// </summary>
   public static class NullableExtension
   {
      #region Public methods

      /// <summary>
      /// Gets a string representation of a specified object.
      /// </summary>
      /// <param name="o">The object.</param>
      /// <returns>String representation of the object or '(null)' if it is null.</returns>
      /// <remarks>The most convenient usage is to log nullable-variables.</remarks>
      public static string SafeToString(this object o)
      {
         return ((o != null) ? o.ToString() : "(null)");
      }

      #endregion
   }
}

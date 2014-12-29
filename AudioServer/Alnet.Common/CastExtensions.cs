#region Copyright

// (c) 2014 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

namespace Alnet.Common
{
   /// <summary>
   /// Provide extension methods for cast types. Use for simplify a code format.
   /// </summary>
   public static class CastExtensions
   {
      /// <summary>
      /// Equivalent: ((TValue)value)
      /// </summary>
      public static TValue To<TValue>(this object value)
      {
         return (TValue)value;
      }

      /// <summary>
      /// Equivalent: (value as TValue)
      /// </summary>
      public static TValue As<TValue>(this object value)
         where TValue : class 
      {
         return value as TValue;
      }

      /// <summary>
      /// Equivalent: (value is TValue)
      /// </summary>
      public static bool Is<TValue>(this object value)
      {
         return value is TValue;
      }
   }
}

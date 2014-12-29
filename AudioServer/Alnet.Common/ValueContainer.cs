#region Copyright

// © 2014 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Diagnostics;

namespace Alnet.Common
{
   /// <summary>
   /// Struct used as a value container.
   /// </summary>
   /// <typeparam name="T">Value type.</typeparam>
   /// <example>
   /// <![CDATA[
   /// public ValueContainer<List<int>> ModifyData(List<int> currentData)
   /// {
   ///   ValueContainer<List<int>> result = ValueContainer<List<int>>.Empty;
   ///   // Check if the data needs to be modified.
   ///   if (currentData.Count > 5)
   ///   {
   ///      result = new ValueContainer<List<int>>(new List<int>(currentData) { 25 });
   ///   }
   ///   return result;
   /// }
   /// 
   /// var data = ModifyData(currentData);
   /// if (data.HasValue)
   /// {
   /// // store data.Value which can possibly be null.
   /// }
   /// ]]>
   /// </example>
   public struct ValueContainer<T>
   {
      /// <summary>
      /// Field for <see cref="Value"/> property.
      /// </summary>
      private readonly T _value;


      /// <summary>
      /// Field for <see cref="HasValue"/> property.
      /// </summary>
      private readonly bool _hasValue;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueContainer{T}"/>. 
      /// </summary>
      /// <param name="value"></param>
      /// <remarks>If this constructor is called then the container is supposed to have a value and <see cref="HasValue"/> will be true.</remarks>
      public ValueContainer(T value)
         : this()
      {
         _value = value;
         _hasValue = true;
      }

      /// <summary>
      /// Value.
      /// </summary>
      /// <remarks>If <see cref="HasValue"/> is <see langword="false"/> the value is undefined and an attempt to access it will throw an <see cref="InvalidOperationException"/>.</remarks>
      public T Value
      {
         get
         {
            if (!HasValue)
            {
               throw new InvalidOperationException("Access to undefind value of ValueContainer occured.");
            }
            return _value;
         }
      }

      /// <summary>
      /// Flag informing weather the container has a value. 
      /// </summary>
      /// <remarks>
      /// <see cref="Value"/> property is supposed to have a meaningfull value iff this property is true.
      /// </remarks>
      public bool HasValue { get { return _hasValue; } }

      /// <summary>
      /// Empty container
      /// </summary>
      public static ValueContainer<T> Empty
      {
         [DebuggerStepThrough]
         get { return new ValueContainer<T>(); }
      }

      /// <summary>
      /// Implements a to-string conversion.
      /// </summary>
      public override string ToString()
      {
         return HasValue ? Value.ToString() : "novalue";
      }
   }
}

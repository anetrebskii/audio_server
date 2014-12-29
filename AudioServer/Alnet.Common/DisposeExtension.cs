#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   /// Extensions and helpers for <see cref="IDisposable"/> interface.
   /// </summary>
   public static class DisposeExtension
   {
      /// <summary>
      /// Safely dispose object (allows <see langword="null"/> input parameter).
      /// </summary>
      /// <param name="disposable">Object to dispose. Allows <see langword="null"/>.</param>
      public static void SafeDispose([CanBeNull] this IDisposable disposable)
      {
         if (disposable != null)
         {
            disposable.Dispose();
         }
      }

      /// <summary>
      /// Try to dispose object.
      /// </summary>
      /// <param name="obj">Object to dispose.</param>
      public static void DisposeObject<T>([CanBeNull] T obj) where T : class
      {
         var disposable = obj as IDisposable;
         SafeDispose(disposable);
      }

      /// <summary>
      /// Try to dispose object.
      /// </summary>
      /// <param name="obj">Object to dispose.</param>
      public static void DisposeObject(this object obj)
      {
         var disposable = obj as IDisposable;
         SafeDispose(disposable);
      }

      /// <summary>
      /// Disposes objects in the provided collection.
      /// </summary>
      /// <param name="collection">Collection of objects to dispose.</param>
      public static void DisposeCollection<T>([CanBeNull] this IEnumerable<T> collection) where T : class
      {
         if (collection == null)
         {
            return;
         }

         foreach (var item in collection)
         {
            item.DisposeObject();
         }
      }

      /// <summary>
      /// Try to dispose object and set it to <see langword="null"/>.
      /// </summary>
      /// <param name="obj">Object to dispose.</param>
      public static void DisposeObject<T>([CanBeNull] ref T obj) where T : class
      {
         DisposeObject(obj);
         obj = null;
      }

      /// <summary>
      /// Try to dispose object.
      /// </summary>
      /// <param name="obj">Object to dispose.</param>
      public static void DisposeObject(ref object obj)
      {
         var disposable = obj as IDisposable;
         SafeDispose(disposable);
         obj = null;
      }
   }
}

#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CCTV.Framework.Utility;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   ///   Extension methods for collections.
   /// </summary>
   public static class CollectionExtension
   {
      /// <summary>
      ///   Enumerates not null elements of <paramref name="collection" />, if it is <see langword="null"/>, empty collection will be returned.
      /// </summary>
      /// <typeparam name="T"> Collections item type. </typeparam>
      /// <param name="collection"> Collection. </param>
      /// <returns> Not null collection. </returns>
      public static IEnumerable<T> NotNull<T>([CanBeNull] this IEnumerable<T> collection) where T : class
      {
         return collection == null ? new T[0] : collection.Where(item => item != null);
      }

      /// <summary>
      /// Perform action for each element in collection.
      /// </summary>
      /// <typeparam name="T">Collections item type.</typeparam>
      /// <param name="collection">Collection to process.</param>
      /// <param name="action">Action to execute.</param>
      /// <remarks>
      /// This method have several performance issues in compare with traditional <see langword="foreach"/> keyword.<br/>
      /// See http://blogs.msdn.com/b/mazhou/archive/2011/09/21/why-no-foreach-method-on-ienumerable-interfaces.aspx">See for details.
      /// </remarks>
      public static void ForEach<T>([CanBeNull] this IEnumerable<T> collection, [NotNull] Action<T> action)
      {
         Guard.VerifyArgumentNotNull(action, "action");
         if (collection == null)
         {
            return;
         }
         foreach (var item in collection)
         {
            action(item);
         }
      }

      /// <summary>
      /// Gets discriminate items from two collections.
      /// </summary>
      /// <typeparam name="E">Type of collection.</typeparam>
      /// <typeparam name="T">Type of item in the collection.</typeparam>
      /// <param name="left">First collection to find differences.</param>
      /// <param name="right">Second collection to find differences.</param>
      /// <returns></returns>
      public static IEnumerable<T> GetDifference<E, T>(this E left, E right)
         where E : IEnumerable<T>
         where T : IEquatable<T>
      {
         return left.Where(x => !right.Any(x.Equals)).Union(right.Where(x => !left.Any(x.Equals)));
      }

      /// <summary>
      /// Indicates that collections have differences. If the collections have duplicating elements the result is not intuitive.
      /// Please see remarks section.
      /// </summary>
      /// <typeparam name="E">Type of collection.</typeparam>
      /// <typeparam name="T">Type of item in the collection.</typeparam>
      /// <param name="left">First collection to find differences.</param>
      /// <param name="right">Second collection to find differences.</param>
      /// <returns>Returns <see langword="true"/> if there are some difference otherwise <see langword="false"/>.</returns>
      /// <remarks>
      /// If any of the collections has duplicating elements the collections can be considered equal even if the number of any duplicated elements
      /// in each collection is not the same. For example integer collections [1, 2, 2] and [1, 1, 2] are equal.
      /// </remarks>
      public static bool HasDifference<E, T>(this E left, E right)
         where E : IEnumerable<T>
         where T : IEquatable<T>
      {
         return HasDifference<E, T>(left, right, (x, y) => x.Equals(y));
      }

      /// <summary>
      /// Indicates that collections have differences with respect to specified compare function. If the collections have duplicating elements the result is not intuitive.
      /// Please see remarks section.
      /// </summary>
      /// <param name="left">First collection to find differences.</param>
      /// <param name="right">Second collection to find differences.</param>
      /// <param name="equalityFunc">Functor for comparing two elements of sequences returning true iff elements are equal. A non-transitive functor can be used (for example
      /// when elements of collections should be considered equal iff the 'difference' between them is small.).</param>
      /// <typeparam name="E">Type of collection.</typeparam>
      /// <typeparam name="T">Type of item in the collection.</typeparam>
      /// <returns>Returns <see langword="true"/> if there are some difference otherwise <see langword="false"/>.</returns>
      /// <remarks>
      /// Collections are supposed to have difference iff they are of different length or there is an element in one of the collections
      /// that doesn't has a corresponding equal (with respect to <paramref name="equalityFunc"/>) element in another collection. 
      /// </remarks>
      public static bool HasDifference<E, T>(this E left, E right, [NotNull]Func<T, T, bool> equalityFunc)
         where E : IEnumerable<T>
      {
         Guard.VerifyArgumentNotNull(equalityFunc, "equalityFunc");

         if (left.Count() != right.Count())
         {
            return true;
         }

         if (left.Any(x => !right.Any(o => equalityFunc(x, o))))
         {
            return true;
         }

         if (right.Any(x => !left.Any(o => equalityFunc(x, o))))
         {
            return true;
         }

         return false;
      }

      /// <summary>
      /// Gets a value by <paramref name="key"/> in <paramref name="storage"/>.
      /// </summary>
      /// <typeparam name="TKey">Key type.</typeparam>
      /// <typeparam name="TValue">Value type.</typeparam>
      /// <param name="storage">Dictionary.</param>
      /// <param name="key">Key value.</param>
      /// <param name="defaultValue">Default value.</param>
      /// <returns>
      /// Value that match the <paramref name="key"/>. <br/>In case there is no <paramref name="key"/> in <paramref name="storage"/> the method returns <paramref name="defaultValue"/>.
      /// </returns>
      public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> storage, TKey key, TValue defaultValue = default(TValue))
      {
         TValue value;
         return storage.TryGetValue(key, out value) ? value : defaultValue;
      }

      /// <summary>
      /// Gets a value by <paramref name="key"/> in <paramref name="storage"/>.
      /// </summary>
      /// <typeparam name="TKey">Key type. Must be a value type.</typeparam>
      /// <typeparam name="TValue">Value type.</typeparam>
      /// <param name="storage">Dictionary.</param>
      /// <param name="key">Key value.</param>
      /// <param name="defaultValue">Default value.</param>
      /// <returns>
      /// Value that match the <paramref name="key"/>. <br/>In case there is no <paramref name="key"/> in <paramref name="storage"/> the method returns <paramref name="defaultValue"/>.
      /// </returns>
      public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> storage, TKey? key, TValue defaultValue = default(TValue)) where TKey : struct
      {
         if (!key.HasValue)
         {
            return defaultValue;
         }

         TValue value;
         return storage.TryGetValue(key.Value, out value) ? value : defaultValue;
      }
   }
}
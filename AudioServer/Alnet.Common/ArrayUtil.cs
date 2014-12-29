using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Alnet.Common.Collections;
using CCTV.Framework.Utility;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   ///   Array utilities.
   /// </summary>
   public static class ArrayUtil
   {
      /// <summary>
      ///   Check to see if the contents of two arrays are equal.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, FALSE otherwise</returns>
      public static bool ArrayEquals(Array lhs, Array rhs)
      {
         if (lhs == rhs)
         {
            return true;
         }

         int len = lhs.Length;
         if (len != rhs.Length)
         {
            return false;
         }

         for (int i = 0; i < len; i++)
         {
            if (!Equals(lhs.GetValue(i), rhs.GetValue(i)))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///   Check to see if the contents of two array lists are equal.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, FALSE otherwise</returns>
      public static bool ArrayListEquals(IList lhs, IList rhs)
      {
         // If references are equal, return true.
         if (lhs == rhs)
         {
            return true;
         }

         if (lhs == null || rhs == null)
         {
            return false;
         }

         int len = lhs.Count;

         // If ArrayLists are different size, return false.
         if (len != rhs.Count)
         {
            return false;
         }

         // Check to see if each of the objects are equal.
         for (int i = 0; i < len; i++)
         {
            if (!Equals(lhs[i], rhs[i]))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///   Join array item to a string by delimiter.
      /// </summary>
      /// <param name = "array">Array to join.</param>
      /// <param name="delimiter">Delimiter strings.</param>
      /// <returns>String representation of array</returns>
      public static string JoinToString(this Array array, string delimiter)
      {
         var sb = new StringBuilder();
         if (array != null && array.Length > 0)
         {
            foreach (var b in array)
            {
               if (sb.Length > 0)
               {
                  sb.Append(delimiter);
               }
               sb.AppendFormat("{0}, ", b);
            }
         }
         return sb.ToString();
      }


      /// <summary>
      ///   Check to see if the contents of two generic lists are equal.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, FALSE otherwise</returns>
      public static bool ListEquals<T>(IList<T> lhs, IList<T> rhs)
      {
         // If references are equal, return true.
         if (lhs == rhs)
         {
            return true;
         }

         if (lhs == null || rhs == null)
         {
            return false;
         }

         int len = lhs.Count;

         // If ArrayLists are different size, return false.
         if (len != rhs.Count)
         {
            return false;
         }

         // Check to see if each of the objects are equal.
         for (int i = 0; i < len; i++)
         {
            if (!Equals(lhs[i], rhs[i]))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///   Check to see if the items of one list contains in other list.
      /// </summary>
      /// <returns><see langword="true"/> if items are equal, <see langword="false"/> otherwise</returns>
      public static bool CheckListContainsEqualItems<T>(IList<T> lhs, IList<T> rhs)
      {
         // If references are equal, return true.
         if (lhs == rhs)
         {
            return true;
         }

         if (lhs == null || rhs == null)
         {
            return false;
         }

         int len = lhs.Count;

         // If ArrayLists are different size, return false.
         if (len != rhs.Count)
         {
            return false;
         }

         // Check to see if each of the objects are equal.
         for (int i = 0; i < len; i++)
         {
            var leftListItem = lhs[i];
            var rightListItem = rhs[i];
            if (!lhs.Contains(rightListItem) && !rhs.Contains(leftListItem))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      /// Check to see if the contents of two generic enumerables are equal.
      /// </summary>
      /// <param name="lhs">First collection.</param>
      /// <param name="rhs">Second collection.</param>
      /// <param name="asEqualFunc">Function which returns value indicating whether 2 elements are equal.</param>
      /// <returns>
      ///    <see langword="true"/> if contents are equal, FALSE otherwise
      /// </returns>
      public static bool EnumerableEquals<T>(
         [CanBeNull] IEnumerable<T> lhs,
         [CanBeNull] IEnumerable<T> rhs,
         [CanBeNull] Func<T, T, bool> asEqualFunc = null)
      {
         asEqualFunc = asEqualFunc ?? ((t1, t2) => Equals(t1, t2));

         // If references are equal, return true.
         if (ReferenceEquals(lhs, rhs))
         {
            return true;
         }

         if (lhs == null || rhs == null)
         {
            return false;
         }

         var lEnumerator = lhs.GetEnumerator();
         var rEnumerator = rhs.GetEnumerator();

         while (true)
         {
            bool lHasValues = lEnumerator.MoveNext();
            bool rHasValues = rEnumerator.MoveNext();

            if (lHasValues != rHasValues)
            {
               return false;
            }

            if (!lHasValues)
            {
               return true;
            }

            if (!asEqualFunc(lEnumerator.Current, rEnumerator.Current))
            {
               return false;
            }
         }
      }

      /// <summary>
      ///   Checks if the contents of two generic enumerables are equal.
      ///   The order of elements is not important.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, FALSE otherwise.</returns>
      public static bool SetEquals<T>(IEnumerable<T> lhs, IEnumerable<T> rhs)
      {
         if (ReferenceEquals(lhs, rhs))
         {
            return true;
         }
         if (lhs == null || rhs == null)
         {
            return false;
         }

         IList<T> rhsUnmatched = new List<T>(rhs);
         IEnumerator<T> lEnumerator = lhs.GetEnumerator();
         while (lEnumerator.MoveNext())
         {
            int i;
            for (i = 0; i < rhsUnmatched.Count; i++)
            {
               if (Equals(lEnumerator.Current, rhsUnmatched[i]))
               {
                  break;
               }
            }
            if (i >= rhsUnmatched.Count)
            {
               return false;
            }
            rhsUnmatched.RemoveAt(i);
         }
         return rhsUnmatched.Count == 0;
      }

      /// <summary>
      ///   Check to see if the contents of two generic dictionaries are equal.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, FALSE otherwise</returns>
      public static bool DictionariesEquals<TKey, TValue>(IDictionary<TKey, TValue> lhs, IDictionary<TKey, TValue> rhs)
      {
         // If references are equal, return true.
         if (lhs == rhs)
         {
            return true;
         }

         if (lhs == null || rhs == null)
         {
            return false;
         }

         if (lhs.Count != rhs.Count)
         {
            return false;
         }

         foreach (var pair in lhs)
         {
            TValue rValue;
            if (!rhs.TryGetValue(pair.Key, out rValue))
            {
               return false;
            }

            if (!Equals(pair.Value, rValue))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///   Checks if the contents of two ordered dictionaries are equal.
      /// </summary>
      /// <returns><see langword="true"/> if contents are equal, false otherwise.</returns>
      public static bool OrderedDictionaryEquals(OrderedDictionary lhs, OrderedDictionary rhs)
      {
         return orderedDictionaryEquals(lhs, rhs);
      }

      /// <summary>
      ///   Check to see if the contents of two SortedLists are equal.
      /// </summary>
      /// <returns>true if contents are equal, false otherwise.</returns>
      public static bool SortedListsEqual(SortedList lhs, SortedList rhs)
      {
         return orderedDictionaryEquals(lhs, rhs);
      }

      /// <summary>
      ///   Checks if the contents of two ordered dictionaries are equal.
      /// </summary>
      /// <returns>true if contents are equal, false otherwise.</returns>
      private static bool orderedDictionaryEquals(IDictionary lhs, IDictionary rhs)
      {
         if (lhs == rhs)
         {
            return true;
         }
         if (lhs == null || rhs == null)
         {
            return false;
         }
         if (lhs.Count != rhs.Count)
         {
            return false;
         }

         // Check to see if each of the objects are equal.
         var lde = lhs.GetEnumerator();
         var rde = rhs.GetEnumerator();
         while (lde.MoveNext() && rde.MoveNext())
         {
            if (!lde.Key.Equals(rde.Key) || !lde.Value.Equals(rde.Value))
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      ///   Check if <c>count</c> of elements in two arrays are equal starting from particular offsets
      /// </summary>
      public static bool ArrayPartsEquals(Array array1, long offset1, Array array2, long offset2, long count)
      {
         if (offset1 + count > array1.Length || offset2 + count > array2.Length)
         {
            return false;
         }

         for (int counter = 0; counter < count; counter++)
         {
            if (!Equals(array1.GetValue(offset1 + counter), array2.GetValue(offset2 + counter)))
            {
               return false;
            }
         }
         return true;
      }

      /// <summary>
      ///   Returns array that is combination of two supplied arrays
      /// </summary>
      /// <remarks>
      ///   The assumption is that both array have same types, otherwise it is exception
      ///   One of the arrays could be zero-length
      /// </remarks>
      public static Array CombineArrays(Array array1, Array array2)
      {
         Guard.VerifyArgumentNotNull(array1, "array1");
         Guard.VerifyArgumentNotNull(array2, "array2");
         Guard.VerifyArgument(array1.Length != 0, "array1 has no elements");
         Guard.VerifyArgument(array2.Length != 0, "array1 has no elements");
         Guard.VerifyArgument(array1.GetValue(0).GetType() == array2.GetValue(0).GetType(), "array1 and array2 has different element types");
         
         var result = Array.CreateInstance(array1.GetValue(0).GetType(), array1.Length + array2.Length);

         array1.CopyTo(result, 0);
         array2.CopyTo(result, array1.Length);
         return result;
      }

      /// <summary>
      ///   Creates string that is hex representation of bytes in byte array separated by spaces
      ///   and with new line character after every 32nd byte.
      /// </summary>
      /// <param name = "data">Byte array to show contents of.</param>
      /// <param name = "bytesToShow">Only so many bytes will be shown in resulting string.
      ///   If this number is larger than the length of the byte array, then the length is used instead.</param>
      /// <returns>String containing hexadecimal view of bytes.</returns>
      public static string ByteArrayAsHexString(byte[] data, int bytesToShow)
      {
         if (data == null)
         {
            return "";
         }

         if (data.Length <= bytesToShow)
         {
            bytesToShow = data.Length;
         }
         var builder = new StringBuilder((bytesToShow*3) + 3);
         for (int i = 0; i < bytesToShow; i++)
         {
            if (i > 0)
            {
               builder.Append(((i%32) == 0) ? "\n" : " ");
            }
            builder.AppendFormat("{0:X2}", data[i]);
         }
         if (bytesToShow < data.Length)
         {
            builder.Append("...");
         }

         return builder.ToString();
      }

      /// <summary>
      ///   Creates string that is hex representation of bytes in byte array separated by spaces
      ///   and with new line character after every 32nd byte.
      /// </summary>
      /// <param name = "data">Byte array to show contents of.</param>
      /// <returns>String containing hexadecimal view of bytes.</returns>
      public static string ByteArrayAsHexString(byte[] data)
      {
         return ByteArrayAsHexString(data, data == null ? 0 : data.Length);
      }

      /// <summary>
      ///   Converts an array of bytes to a human readable string
      /// </summary>
      /// <param name = "bytes">byte array to show contents of</param>
      /// <returns>String representation of byte array</returns>
      public static string ByteArrayToString(byte[] bytes)
      {
         var sb = new StringBuilder();
         if (bytes != null && bytes.Length > 0)
         {
            foreach (var b in bytes)
            {
               sb.AppendFormat("{0}, ", b);
            }
         }
         else
         {
            sb.Append("null or empty byte array");
         }
         return sb.ToString();
      }



      /// <summary>
      ///   Check if <c>count</c> and contents of bytes in two byte arrays are equal starting from particular offsets
      /// </summary>
      public static bool ByteArrayPartsEquals(byte[] array1, int offset1, byte[] array2, int offset2, int count)
      {
         if (offset1 + count > array1.Length || offset2 + count > array2.Length)
         {
            return false;
         }

         for (int counter = 0; counter < count; counter++)
         {
            if (array1[offset1 + counter] != array2[offset2 + counter])
            {
               return false;
            }
         }
         return true;
      }

      /// <summary>
      ///   Slices a part from the source array.
      /// </summary>
      /// <param name = "array">Source array.</param>
      /// <param name = "offset">Offset in the source array.</param>
      /// <param name = "length">Length of a slice.</param>
      /// <returns>A part of the source array as the new array, or source array if
      ///   <paramref name = "offset" /> and <paramref name = "length" /> specify entire source array.</returns>
      public static byte[] Slice(byte[] array, int offset, int length)
      {
         if (offset == 0 && length == array.Length)
         {
            return array;
         }

         var slice = new byte[length];
         Buffer.BlockCopy(array, offset, slice, 0, length);
         return slice;
      }

      /// <summary>
      /// Adds element to the specified collection is sorted order.
      /// </summary>
      /// <typeparam name="T">Type of element.</typeparam>
      /// <param name="collection">Collection.</param>
      /// <param name="elementToAdd">Element to add to the collection.</param>
      /// <param name="comparison">Comparison rule for elements in the collection.</param>
      public static void AddSorted<T>(
         this IList<T> collection,
         T elementToAdd,
         Comparison<T> comparison)
      {

         Guard.VerifyArgumentNotNull(collection, "collection");
         Guard.VerifyArgumentNotNull(comparison, "comparison");

         T foundElement = collection.FirstOrDefault(element => 0 < comparison(element, elementToAdd));
         int index = collection.IndexOf(foundElement);
         if (index < 0)
         {
            collection.Add(elementToAdd);
         }
         else
         {
            collection.Insert(index, elementToAdd);
         }
      }

      /// <summary>
      /// Remove last items.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Removed count.</param>
      public static void RemoveLastItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list,"list");
         if (count <= 0)
         {
            return;
         }
         count = Math.Min(count, list.Count);
         list.RemoveRange(list.Count - count,count);
      }

      /// <summary>
      /// Remove first items.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Removed count.</param>
      public static void RemoveFirstItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list, "list");
         if (count <= 0)
         {
            return;
         }
         list.RemoveRange(0, Math.Min(count, list.Count));
      }

      /// <summary>
      /// Save last items. Other removed.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Saved count.</param>
      public static void SaveOnlyLastItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list, "list");
         if (count < 0 || count >= list.Count)
         {
            return;
         }
         list.RemoveFirstItems(list.Count - count);
      }

      /// <summary>
      /// Save first items. Other removed.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Saved count.</param>
      public static void SaveOnlyFirstItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list, "list");
         if (count < 0 || count >= list.Count)
         {
            return;
         }
         list.RemoveLastItems(list.Count - count);
      }

      /// <summary>
      /// Get last items.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Range count.</param>
      public static List<T> GetLastItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list, "list");
         if (count <= 0)
         {
            return new List<T>();
         }
         count = Math.Min(count, list.Count);
         return list.GetRange(list.Count - count, count);
      }

      /// <summary>
      /// Get first items.
      /// </summary>
      /// <typeparam name="T">Type of list items.</typeparam>
      /// <param name="list">List.</param>
      /// <param name="count">Range count.</param>
      public static List<T> GetFirstItems<T>(this List<T> list, int count)
      {
         Guard.VerifyArgumentNotNull(list, "list");
         if (count <= 0)
         {
            return new List<T>();
         }
         return list.GetRange(0, Math.Min(count, list.Count));
      }

      /// <summary>
      /// Get new or modified items for currentDictionary compare with oldDictionary.
      /// </summary>
      /// <typeparam name="TKey">Key type.</typeparam>
      /// <typeparam name="TValue">Value type.</typeparam>
      /// <param name="currentDictionary">Current dictionary,</param>
      /// <param name="oldDictionary">Old dictionary.</param>
      public static IDictionary<TKey, TValue> GetNewOrModifiedItems<TKey, TValue>(this IDictionary<TKey, TValue> currentDictionary, IDictionary<TKey, TValue> oldDictionary)
      {
         if (currentDictionary == null)
         {
            return null;
         }
         if (oldDictionary == null)
         {
            return currentDictionary;
         }
         var changesDictionary = new Dictionary<TKey, TValue>();
         foreach (var keyValuePair in currentDictionary)
         {
            TValue oldValue;
            if (!oldDictionary.TryGetValue(keyValuePair.Key, out oldValue) || !Equals(oldValue,keyValuePair.Value))
            {
               changesDictionary.Add(keyValuePair.Key,keyValuePair.Value);
            }
         }
         return changesDictionary;
      }

      /// <summary>
      /// Get union of two collection with notify.
      /// </summary>
      /// <typeparam name="TElement">Type of element.</typeparam>
      /// <param name="firstCollection">First collection.</param>
      /// <param name="secondCollection">Second collection.</param>
      public static IEnumerable<TElement> UnionWithNotify<TElement>(this IEnumerable<TElement> firstCollection, IEnumerable<TElement> secondCollection)
      {
         return new UnionNotifyCollection<TElement>(firstCollection, secondCollection);
      }

      /// <summary>
      /// Get union of two collection with notify.
      /// </summary>
      /// <typeparam name="TElement">Type of element.</typeparam>
      /// <param name="firstCollection">First collection.</param>
      /// <param name="secondCollection">Second collection.</param>
      /// <param name="autoDisposeFirstCollection">Auto dispose first collection. </param>
      public static IEnumerable<TElement> UnionWithNotify<TElement>(this IEnumerable<TElement> firstCollection, IEnumerable<TElement> secondCollection, bool autoDisposeFirstCollection)
      {
         return new UnionNotifyCollection<TElement>(firstCollection, secondCollection, autoDisposeFirstCollection);
      }

      /// <summary>
      /// Get union of two collection with notify.
      /// </summary>
      /// <typeparam name="TElement">Type of element.</typeparam>
      /// <param name="firstCollection">First collection.</param>
      /// <param name="secondCollection">Second collection.</param>
      /// <param name="autoDisposeFirstCollection">Auto dispose first collection. </param>
      /// <param name="autoDisposeSecondCollection">Auto dispose second collection.  </param>
      public static IEnumerable<TElement> UnionWithNotify<TElement>(this IEnumerable<TElement> firstCollection, IEnumerable<TElement> secondCollection, bool autoDisposeFirstCollection, bool autoDisposeSecondCollection)
      {
         return new UnionNotifyCollection<TElement>(firstCollection, secondCollection, autoDisposeFirstCollection, autoDisposeSecondCollection);
      }
   }
}
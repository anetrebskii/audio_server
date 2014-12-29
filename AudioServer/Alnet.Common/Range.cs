#region Copyright

// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Alnet.Common;
using JetBrains.Annotations;

namespace CCTV.Framework.Utility
{
   /// <summary>
   /// 	Describe comparable range.
   /// </summary>
   /// <typeparam name="TBound"> Type of range boundaries. </typeparam>
   public class Range<TBound> where TBound : IComparable<TBound>
   {
      #region Static fields

      #endregion

      #region Fields

      /// <summary>
      /// 	Max bound.
      /// </summary>
      private TBound _maxBound;

      /// <summary>
      /// 	Min bound.
      /// </summary>
      private TBound _minBound;

      #endregion

      #region Constructors

      /// <summary>
      /// 	Initializes a new instance of <see cref="Range{TBound}" /> class.
      /// </summary>
      /// <param name="minimum"> Minimum bound. </param>
      /// <param name="maximum"> Maximum bound. </param>
      [DebuggerStepThrough]
      public Range(TBound minimum, TBound maximum)
      {
         MinBound = Min(minimum, maximum);
         MaxBound = Max(minimum, maximum);
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// 	Check whether region is empty.
      /// </summary>
      public bool IsEmpty
      {
         [Pure] get { return MaxBound.CompareTo(MinBound) == 0; }
      }

      /// <summary>
      /// 	Max bound.
      /// </summary>
      public TBound MaxBound
      {
         [DebuggerStepThrough] get { return _maxBound; }
         set
         {
            if (!Equals(_maxBound, value))
            {
               _maxBound = value;
            }
         }
      }

      /// <summary>
      /// 	Min bound.
      /// </summary>
      public TBound MinBound
      {
         [DebuggerStepThrough] get { return _minBound; }
         set
         {
            if (!Equals(_minBound, value))
            {
               _minBound = value;
            }
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Union fragments.
      /// </summary>
      /// <param name="ranges">Fragments for union.</param>
      /// <returns>Collection of not intersects fragments.</returns>
      public static ICollection<Range<TBound>> Union([NotNull] IEnumerable<Range<TBound>> ranges, bool excludeEmptyRanges = false)
      {
         Guard.VerifyArgumentNotNull(ranges, "ranges");

         if(!ranges.Any())
         {
            return new Range<TBound>[0];
         }

         var result = new List<Range<TBound>>();

         foreach(var range in ranges)
         {
            if(range.IsEmpty && excludeEmptyRanges)
            {
               continue;
            }

            if(result.Count == 0)
            {
               result.Add(new Range<TBound>(range.MinBound, range.MaxBound));
            }
            else
            {
               bool hasIntersections = false;
               foreach(var item in result)
               {
                  if(item.IsIntersects(range))
                  {
                     item.ExtendBounds(range.MinBound);
                     item.ExtendBounds(range.MaxBound);
                     hasIntersections = true;
                  }
               }

               if(!hasIntersections)
               {
                  result.Add(new Range<TBound>(range.MinBound, range.MaxBound));
               }
               else
               {
                  result = Union(result, excludeEmptyRanges).ToList();
               }
            }
         }

         return result;
      }

      /// <summary>
      /// 	Gets minimum intersection of fragments.
      /// </summary>
      /// <param name="range1"> Fragment 1. </param>
      /// <param name="range2"> Fragment 2. </param>
      /// <returns> Minimum intersections. </returns>
      public static Range<TBound> GetIntersection(Range<TBound> range1, Range<TBound> range2)
      {
         if (range1.IsIntersects(range2))
         {
            return new Range<TBound>(Max(range1.MinBound, range2.MinBound), Min(range1.MaxBound, range2.MaxBound));
         }
         return null;
      }

      /// <summary>
      ///   Checks that fragments is intersected. If one of fragments is null then result is FALSE.
      /// </summary>
      /// <param name="range1"> Fragment 1. </param>
      /// <param name="range2"> Fragment 2. </param>
      /// <returns> Flag that fragments has intersection. </returns>
      [Pure]
      public static bool AreIntersects(Range<TBound> range1, Range<TBound> range2)
      {
         if (range1 == null || range2 == null)
         {
            return false;
         }

         return range1.IsIntersects(range2);
      }

      /// <summary>
      /// 	Gets maximal value.
      /// </summary>
      public static TBound Max(params TBound[] bounds)
      {
         Guard.VerifyArgumentNotNull(bounds, "bounds");

         if (bounds.Length == 0)
         {
            throw new ArgumentException("Bounds should contains more than zero elements.");
         }

         TBound maxBound = bounds[0];
         maxBound = bounds.Aggregate(maxBound, maxFromTwoBounds);

         return maxBound;
      }

      /// <summary>
      /// 	Gets minimal value.
      /// </summary>
      public static TBound Min(params TBound[] bounds)
      {
         Guard.VerifyArgumentNotNull(bounds, "bounds");

         if (bounds.Length == 0)
         {
            throw new ArgumentException("Bounds should contains more than zero elements.");
         }

         TBound minBound = bounds[0];
         minBound = bounds.Aggregate(minBound, minFromTwoBounds);

         return minBound;
      }

      /// <summary>
      /// Check that fragments has the same range.
      /// </summary>
      /// <param name="range"> Fragment for comparasion. </param>
      /// <returns>If ranges has same bounds - true, otherwise - false.</returns>
      [Pure]
      public bool AreTheSame(Range<TBound> range)
      {
         Guard.VerifyArgumentNotNull(range, "range");

         return MinBound.CompareTo(range.MinBound) == 0 && MaxBound.CompareTo(range.MaxBound) == 0;
      }

      /// <summary>
      /// Remove given range from current.
      /// </summary>
      /// <param name="range">Range for removing.</param>
      /// <returns>Ranges available after removing <see cref="range"/>.</returns>
      [Pure]
      public ICollection<Range<TBound>> ExcludeRange(Range<TBound> range)
      {
         if (IsSubset(range))
         {
            return new Range<TBound>[0];
         }

         if (!IsIntersects(range))
         {
            return new[] {this};
         }

         if (range.IsSubset(this))
         {
            return new[] {new Range<TBound>(MinBound, range.MinBound), new Range<TBound>(range.MaxBound, MaxBound)};
         }

         if (MinBound.CompareTo(range.MinBound) == -1)
         {
            return new[] {new Range<TBound>(MinBound, Min(MaxBound, range.MinBound))};
         }

         return new[] {new Range<TBound>(Max(MinBound, range.MaxBound), MaxBound)};
      }

      /// <summary>
      /// Check that current range is subset of <see cref="range"/>.
      /// </summary>
      /// <param name="range">Range for comparision.</param>
      /// <returns>If current range is completely included into <see cref="range"/> returns true, otherwise - false.</returns>
      [Pure]
      public bool IsSubset(Range<TBound> range)
      {
         int maxRangeCompare = MaxBound.CompareTo(range.MaxBound);
         int minRangeCompare = MinBound.CompareTo(range.MinBound);
         return maxRangeCompare <= 0 && minRangeCompare >= 0;
      }

      /// <summary>
      /// Check that given item contains in range interval.
      /// </summary>
      /// <param name="item">Item for checking.</param>
      /// <returns>If item contains in range - true, otherwise - false.</returns>
      [Pure]
      public bool IsInRange(TBound item)
      {
         return MinBound.CompareTo(item) <= 0 && MaxBound.CompareTo(item) >= 0;
      }

      /// <summary>
      /// 	Extends range bounds.
      /// </summary>
      /// <param name="bound"> New bounds to check and if larger - extend current range. </param>
      public void ExtendBounds(TBound bound)
      {
         if (MinBound.CompareTo(bound) >= 0)
         {
            MinBound = bound;
         }

         if (bound.CompareTo(MaxBound) >= 0)
         {
            MaxBound = bound;
         }
      }

      /// <summary>
      /// 	Checks that fragments is intersected.
      /// </summary>
      [Pure]
      public bool IsIntersects(Range<TBound> range)
      {
         return IsIntersects(range.MinBound, range.MaxBound);
      }

      /// <summary>
      /// 	Checks that fragments is intersected.
      /// </summary>
      [Pure]
      public bool IsIntersects(TBound minBound, TBound maxBound)
      {
         if (MinBound.CompareTo(minBound) >= 0 && maxBound.CompareTo(MinBound) >= 0)
         {
            return true;
         }

         if (MaxBound.CompareTo(minBound) >= 0 && maxBound.CompareTo(MaxBound) >= 0)
         {
            return true;
         }

         if (minBound.CompareTo(MinBound) >= 0 && MaxBound.CompareTo(maxBound) >= 0)
         {
            return true;
         }

         return false;
      }

      #endregion

      #region Private methods

      /// <summary>
      /// 	Gets maximal value.
      /// </summary>
      private static TBound maxFromTwoBounds(TBound bound1, TBound bound2)
      {
         if (bound1.CompareTo(bound2) > 0)
         {
            return bound1;
         }

         return bound2;
      }

      /// <summary>
      /// 	Gets minimal value.
      /// </summary>
      public static TBound minFromTwoBounds(TBound bound1, TBound bound2)
      {
         if (bound1.CompareTo(bound2) < 0)
         {
            return bound1;
         }

         return bound2;
      }

      #endregion

      #region Base Overrides

      /// <summary>
      /// Range as sting representer.
      /// </summary>
      /// <returns>String representation of range.</returns>
      public override string ToString()
      {
         return string.Concat("[", _minBound.ToString(), "; ", _maxBound.ToString(), "]");
      }

      #endregion
   }
}
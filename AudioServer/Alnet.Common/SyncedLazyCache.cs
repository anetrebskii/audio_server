#region Usings
#region Copyright
// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.
#endregion

using System;
using System.Diagnostics;
using Alnet.Common;
using JetBrains.Annotations;
#endregion


// ReSharper disable once CheckNamespace
namespace CCTV.Framework.Utility
{
   /// <summary>
   /// Caching proxy for the value from some backing store that allows multi-threaded reading and writing operations.
   /// Write support gives two main difference from thread-safe variant of System.Lazy:
   /// 1. If the value is assigned before any reads then factory function is never called.
   /// 2. Setting value results in value writing both to backing store and to cache and writes are synchronized in the following sense:
   ///   if two threads simultaneously tries to set value,
   ///   the resulting value in this cache and in backing store is the same after all setters return.
   /// </summary>
   /// <typeparam name="T">type to store. Must be class to eliminate value read/write atomicity problems</typeparam>
   public class SyncedLazyCache<T>
      where T : class
   {
      #region Private fields
      /// <summary>
      /// lock used to synchronize write operations
      /// </summary>
      private readonly object _savingLock = new object();

      /// <summary>
      /// factory for creating values or null if object already was initially created.
      /// </summary>
      private Func<T> _valueFactory;

      /// <summary>
      /// cached value. Can be null.
      /// </summary>
      private T _cachedValue;
      #endregion

      #region Constructor
      /// <summary>
      /// creates proxy instance without loading any data.
      /// </summary>
      /// <param name="valueFactory">function to get value from backing store. Used if the first read operation appears before first write operation. Can return null</param>
      public SyncedLazyCache([NotNull] Func<T> valueFactory)
      {
         Guard.VerifyArgumentNotNull(valueFactory, "valueFactory");
         _valueFactory = valueFactory;
      }
      #endregion

      #region Public Properties
      /// <summary>
      /// cached value. Get operation of this property is thread-safe.
      /// Get returns last set value from cache or value if a value was never read or written populates cache from backing store.
      /// </summary>
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      public T Value
      {
         get
         {
            if (!ObjectAlreadyInitialized)
            {
               lock (_savingLock)
               {
                  loadToCacheIfNotYet();
               }
            }
            return _cachedValue;
         }
      }
      #endregion

      #region Private Properties
      /// <summary>
      /// detects if object was already loaded from backing store and cache value is actual
      /// </summary>
      private bool ObjectAlreadyInitialized
      {
         get
         {
            return _valueFactory == null;
         }
      }
      #endregion

      #region Public Methods

      /// <summary>
      /// Modify the cached value. Writing to backing store is performed by specified functor.
      /// </summary>
      /// <param name="oldToNewConfiguration">Function obtaining current stored configuration and returning a container with new one.
      /// If the container is empty or the functor throws an exception the cached value is not modified.</param>
      /// <returns>True if modification was performed.</returns>
      public bool AtomicModify([NotNull] Func<T, ValueContainer<T>> oldToNewConfiguration)
      {
         Guard.VerifyArgumentNotNull(oldToNewConfiguration, "oldToNewConfiguration");

         lock (_savingLock)
         {
            loadToCacheIfNotYet();
            var newConfiguration = oldToNewConfiguration(_cachedValue);
            if (newConfiguration.HasValue)
            {
               _cachedValue = newConfiguration.Value;

               //if object is set before initialization treat value as initialized and remove _valueFactory
               _valueFactory = null;

               return true;
            }
            return false;
         }
      }

      #endregion

      #region Private Methods
      /// <summary>
      /// loads value from storage to cache if it was not already loaded
      /// </summary>
      private void loadToCacheIfNotYet()
      {
         if (!ObjectAlreadyInitialized)
         {
            _cachedValue = _valueFactory();
            _valueFactory = null;
         }
      }

      #endregion
   }
}

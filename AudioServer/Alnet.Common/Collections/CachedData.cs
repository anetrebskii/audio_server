#region Copyright

// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using CCTV.Framework.Utility;

namespace Alnet.Common.Collections
{
   /// <summary>
   /// Represents cached data object. Provides ability to load data and notify if data were changed.
   /// </summary>
   /// <remarks>
   /// Thread safe.
   /// It is good to override <see cref="object.Equals(object)"/> in order to provide correct comapision.
   /// </remarks>
   /// <typeparam name="TKey"> Type of a key for data object. </typeparam>
   /// <typeparam name="TItem"> Type of data object. </typeparam>
   [DebuggerDisplay("Cache: {_cacheName}")]
   public sealed class CachedData<TKey, TItem> : ICachedData where TItem : class 
   {
      #region Fields

      /// <summary>
      /// Name of the cached object.
      /// </summary>
      private readonly string _cacheName;

      /// <summary>
      /// Cached data.
      /// </summary>
      private volatile Dictionary<TKey, TItem> _cachedData = new Dictionary<TKey, TItem>();

      /// <summary>
      /// Notifies about data changes.
      /// </summary>
      private EventHandler<DataEventArgs<string>> _dataChanged;

      /// <summary>
      /// Readonly wrapper for stored data.
      /// </summary>
      private volatile ReadOnlyCollection<TItem> _readonlyCollection;

      /// <summary>
      /// Used to update <see cref="_cachedData"/> and <see cref="_readonlyCollection"/> instances.
      /// </summary>
      private SpinLock _spinLock = new SpinLock(false);

      /// <summary>
      /// Key selector.
      /// </summary>
      private readonly Func<TItem, TKey> _keySelector;

      /// <summary>
      /// Load function to load cached data from storage.
      /// </summary>
      private readonly Func<IEnumerable<TItem>> _loadFunction;

      /// <summary>
      /// Value indicating whether data were loaded.
      /// </summary>
      private volatile bool _isLoaded;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CachedData{TKey,TItem}"/> class.
      /// </summary>
      /// <param name="keySelector">Delegate for key selection.</param>
      /// <param name="loadFunction">Data loading routine.</param>
      /// <param name="cacheName">Name of the chached data.</param>
      public CachedData(Func<TItem, TKey> keySelector, Func<IEnumerable<TItem>> loadFunction, string cacheName)
      {
         _keySelector = Guard.EnsureArgumentNotNull(keySelector);
         _loadFunction = Guard.EnsureArgumentNotNull(loadFunction);
         _cacheName = Guard.EnsureArgumentNotNullOrEmpty(cacheName, "cacheName");

         // Prevents NullReferenceExceptions.
         _readonlyCollection = new ReadOnlyCollection<TItem>(new List<TItem>());
      }

      #endregion

      #region Events

      /// <summary>
      /// Notifies about data changes.
      /// </summary>
      /// <remarks>
      /// Will called from caller thread. (May not be from UI thread).
      /// </remarks>
      public event EventHandler<DataEventArgs<string>> DataChanged
      {
         add
         {
            bool lockObtained = false;
            try
            {
               _spinLock.Enter(ref lockObtained);
               _dataChanged += value;
            }
            finally
            {
               if (lockObtained)
               {
                  _spinLock.Exit();
               }
            }
         }
         remove
         {
            bool lockObtained = false;
            try
            {
               _spinLock.Enter(ref lockObtained);
               _dataChanged -= value;
            }
            finally
            {
               if (lockObtained)
               {
                  _spinLock.Exit();
               }
            }
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Load data and if loaded data differs from stored one - save it and notify about changes.
      /// </summary>
      public bool Load()
      {
         // Load
         var loadedData = _loadFunction().ToList();

         // Compare data.
         if (CompareData(loadedData))
         {
            return false;
         }

         var newDictionary = new Dictionary<TKey, TItem>();
         foreach (TItem item in loadedData)
         {
            newDictionary.Add(_keySelector(item), item);
         }

         updateCachedData(newDictionary, new ReadOnlyCollection<TItem>(loadedData));
         
         _isLoaded = true;

         return true;
      }

      /// <summary>
      /// Rises event.
      /// </summary>
      public void RiseEvent()
      {
         // Rise event.
         EventsHelper.InvokeEventHandler(_dataChanged, this, new DataEventArgs<string>(_cacheName));
      }

      /// <summary>
      /// Gets cached data.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <returns>Stored data or <see langref="null"/>.</returns>
      [Pure]
      public TItem GetItem(TKey key)
      {
         TItem ret;

         var current = getDictionary();

         if (!current.TryGetValue(key, out ret))
         {
            return default(TItem);
         }

         return ret;
      }


      /// <summary>
      /// Compares loaded data with stored one.
      /// </summary>
      /// <param name="loadedData"> Loaded data. </param>
      /// <returns> <see langword="true" /> if data identical. </returns>
      public bool CompareData(IEnumerable<TItem> loadedData)
      {
         var current = getDictionary();
         var currentCount = current.Count;
         int loadedCount = 0;
         foreach (var item in loadedData)
         {
            loadedCount++;
            TItem cachedItem;

            var key = _keySelector(item);

            if (!current.TryGetValue(key, out cachedItem))
            {
               return false;
            }

            if (!Equals(item, cachedItem))
            {
               return false;
            }
         }
         return loadedCount == currentCount;
      }

      #endregion

      #region Public properties

      /// <summary>
      /// Gets all stored items.
      /// </summary>
      [Pure]
      public IReadOnlyCollection<TItem> Items
      {
         [DebuggerStepThrough]
         get { return getCollection(); }
      }

      /// <summary>
      /// Gets all stored items as a dictionary.
      /// </summary>
      [Pure]
      public IReadOnlyDictionary<TKey, TItem> Dictionary
      {
         [DebuggerStepThrough]
         get { return getDictionary(); }
      }
      
         /// <summary>
      /// Value indicating whether data were loaded into cache.
      /// </summary>
      [Pure]
      public bool IsLoaded
      {
         [DebuggerStepThrough]
         get { return _isLoaded; }
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Get reference to current instance of <see cref="_cachedData"/>.
      /// </summary>
      /// <returns>Current instance of collection.</returns>
      private IReadOnlyDictionary<TKey, TItem> getDictionary()
      {
         bool lockObtained = false;
         try
         {
            _spinLock.Enter(ref lockObtained);
            var items = _cachedData;
            return items;
         }
         finally
         {
            if (lockObtained)
            {
               _spinLock.Exit();
            }
         }
      }

      /// <summary>
      /// Get reference to current instance of <see cref="_readonlyCollection"/>.
      /// </summary>
      /// <returns>Current instance of collection.</returns>
      private IReadOnlyCollection<TItem> getCollection()
      {
         bool lockObtained = false;
         try
         {
            _spinLock.Enter(ref lockObtained);
            var items = _readonlyCollection;
            return items;
         }
         finally
         {
            if (lockObtained)
            {
               _spinLock.Exit();
            }
         }
      }

      /// <summary>
      /// Update reference to collection of <see cref="_cachedData"/> and <see cref="_readonlyCollection"/>.
      /// </summary>
      private void updateCachedData(Dictionary<TKey, TItem> newDictionary, ReadOnlyCollection<TItem> newCollection)
      {
         bool lockObtained = false;
         try
         {
            _spinLock.Enter(ref lockObtained);
            _cachedData = newDictionary;
            _readonlyCollection = newCollection;
         }
         finally
         {
            if (lockObtained)
            {
               _spinLock.Exit();
            }
         }
      }
      #endregion
   }
}
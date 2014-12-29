#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CCTV.Framework.Utility;
using JetBrains.Annotations;

#endregion

namespace Alnet.Common.Collections
{
   /// <summary>
   ///    Union notify collection.
   /// </summary>
   /// <typeparam name="TElement"> Type of element. </typeparam>
   internal class UnionNotifyCollection<TElement> : IDisposable, IEnumerable<TElement>, ICollection, INotifyCollectionChanged
   {
      #region Private fields

      /// <summary>
      /// Auto dispose first collection.
      /// </summary>
      private readonly bool _autoDisposeFirstCollection;

      /// <summary>
      /// Auto dispose second collection.
      /// </summary>
      private readonly bool _autoDisposeSecondCollection;

      /// <summary>
      ///    First collection.
      /// </summary>
      [NotNull]
      private IEnumerable<TElement> _firstCollection;

      /// <summary>
      ///    Second collection.
      /// </summary>
      [NotNull]
      private IEnumerable<TElement> _secondCollection;

      /// <summary>
      ///    First collection count.
      /// </summary>
      private int _firstCollectionCount;

      /// <summary>
      ///    Second collection count.
      /// </summary>
      private int _secondCollectionCount;

      /// <summary>
      /// Instance of <see cref="DisposedGuard"/>.
      /// </summary>
      [NotNull]
      private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(UnionNotifyCollection<>));

      /// <summary>
      /// Temporary removed items that should not be used in enumerations.
      /// </summary>
      private List<TElement> _removedItemsCache;

      #endregion

      #region Constructors

      /// <summary>
      ///    Ctor for <see cref="UnionNotifyCollection{TElement}" />.
      /// </summary>
      /// <param name="firstCollection"> First collection. </param>
      /// <param name="secondCollection"> Second collection. </param>
      public UnionNotifyCollection([NotNull] IEnumerable<TElement> firstCollection, [NotNull] IEnumerable<TElement> secondCollection)
      {
         _firstCollection = Guard.EnsureArgumentNotNull(firstCollection, "firstCollection");
         _secondCollection = Guard.EnsureArgumentNotNull(secondCollection, "secondCollection");
         _firstCollectionCount = _firstCollection.Count();
         _secondCollectionCount = _secondCollection.Count();

         var firstNotifyCollection = firstCollection as INotifyCollectionChanged;
         if (firstNotifyCollection != null)
         {
            firstNotifyCollection.CollectionChanged += onFirstCollectionChanged;
         }

         var secondNotifyCollection = secondCollection as INotifyCollectionChanged;
         if (secondNotifyCollection != null)
         {
            secondNotifyCollection.CollectionChanged += onSecondCollectionChanged;
         }
      }

      /// <summary>
      ///    Ctor for <see cref="UnionNotifyCollection{TElement}" />.
      /// </summary>
      /// <param name="firstCollection"> First collection. </param>
      /// <param name="secondCollection"> Second collection. </param>
      /// <param name="autoDisposeFirstCollection">Auto dispose first collection. </param>
      public UnionNotifyCollection([NotNull] IEnumerable<TElement> firstCollection, [NotNull] IEnumerable<TElement> secondCollection, bool autoDisposeFirstCollection):this(firstCollection,secondCollection)
      {
         _autoDisposeFirstCollection = autoDisposeFirstCollection;
      }

      /// <summary>
      ///    Ctor for <see cref="UnionNotifyCollection{TElement}" />.
      /// </summary>
      /// <param name="firstCollection"> First collection. </param>
      /// <param name="secondCollection"> Second collection. </param>
      /// <param name="autoDisposeFirstCollection">Auto dispose first collection. </param>
      /// <param name="autoDisposeSecondCollection">Auto dispose second collection.  </param>
      public UnionNotifyCollection([NotNull] IEnumerable<TElement> firstCollection, [NotNull] IEnumerable<TElement> secondCollection, bool autoDisposeFirstCollection, bool autoDisposeSecondCollection)
         : this(firstCollection, secondCollection,autoDisposeFirstCollection)
      {
         _autoDisposeSecondCollection = autoDisposeSecondCollection;
      }

      #endregion

      #region Private Methods

      /// <summary>
      ///    First collection changed.
      /// </summary>
      /// <param name="sender">
      ///    Instance of <see cref="INotifyCollectionChanged" />.
      /// </param>
      /// <param name="e">
      ///    <see cref="NotifyCollectionChangedEventArgs" /> argument.
      /// </param>
      private void onFirstCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(_disposedGuard.IsDisposed)
         {
            return;
         }

         switch (e.Action)
         {
            case NotifyCollectionChangedAction.Add:
               if (e.NewItems != null)
               {
                  _firstCollectionCount += e.NewItems.Count;
               }
               break;
            case NotifyCollectionChangedAction.Remove:
               if (e.OldItems != null)
               {
                  _firstCollectionCount -= e.OldItems.Count;
               }
               break;
            case NotifyCollectionChangedAction.Reset:
               _removedItemsCache = new List<TElement>();

               var firstCollection = _firstCollection.ToList();
               for(int i = firstCollection.Count - 1; i >= 0; i--)
               {
                  _firstCollectionCount--;
                  _removedItemsCache.Add(firstCollection[i]);

                  EventsHelper.SafeInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, firstCollection[i], i));
               }

               _removedItemsCache = null;
               return;
         }
         EventsHelper.SafeInvoke(CollectionChanged, this, e);
      }

      /// <summary>
      ///    Second collection changed.
      /// </summary>
      /// <param name="sender">
      ///    Instance of <see cref="INotifyCollectionChanged" />.
      /// </param>
      /// <param name="e">
      ///    <see cref="NotifyCollectionChangedEventArgs" /> argument.
      /// </param>
      private void onSecondCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(_disposedGuard.IsDisposed)
         {
            return;
         }

         NotifyCollectionChangedEventArgs newArgs;
         switch (e.Action)
         {
            case NotifyCollectionChangedAction.Add:
               if (e.NewItems != null)
               {
                  _secondCollectionCount += e.NewItems.Count;
               }
               newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex == -1 ? -1 : e.NewStartingIndex + _firstCollectionCount);
               break;
            case NotifyCollectionChangedAction.Remove:
               if (e.OldItems != null)
               {
                  _secondCollectionCount -= e.OldItems.Count;
               }
               newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex == -1 ? -1 : e.OldStartingIndex + _firstCollectionCount);
               break;
            case NotifyCollectionChangedAction.Replace:
               newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                              e.NewItems,
                                                              e.OldItems,
                                                              e.OldStartingIndex == -1 ? -1 : e.OldStartingIndex + _firstCollectionCount);
               break;
            case NotifyCollectionChangedAction.Move:
               newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,
                                                              e.NewItems,
                                                              e.NewStartingIndex == -1 ? -1 : e.NewStartingIndex + _firstCollectionCount,
                                                              e.OldStartingIndex == -1 ? -1 : e.OldStartingIndex + _firstCollectionCount);
               break;
            case NotifyCollectionChangedAction.Reset:
               _removedItemsCache = new List<TElement>();

               var secondCollection = _secondCollection.ToList();
               for(int i = secondCollection.Count - 1; i >= 0; i--)
               {
                  _secondCollectionCount--;
                  _removedItemsCache.Add(secondCollection[i]);

                  EventsHelper.SafeInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, secondCollection[i], i + _firstCollectionCount));
               }

               _removedItemsCache = null;
               return;
            default:
               return;
         }
         EventsHelper.SafeInvoke(CollectionChanged, this, newArgs);
      }

      #endregion

      #region ICollection members

      /// <summary>
      ///    Implements for <see cref="ICollection.CopyTo" />.
      /// </summary>
      void ICollection.CopyTo(Array array, int index)
      {
         _disposedGuard.Check();
         new List<TElement>(this).CopyTo((TElement[])array, index);
      }

      /// <summary>
      ///    Implements for <see cref="ICollection.Count" />.
      /// </summary>
      int ICollection.Count
      {
         get { return _firstCollectionCount + _secondCollectionCount; }
      }

      /// <summary>
      ///    Implements for <see cref="ICollection.SyncRoot" />.
      /// </summary>
      object ICollection.SyncRoot
      {
         get { return this; }
      }

      /// <summary>
      ///    Implements for <see cref="ICollection.IsSynchronized" />.
      /// </summary>
      bool ICollection.IsSynchronized
      {
         get { return false; }
      }

      #endregion

      #region IDisposable members

      /// <summary>
      ///    Implements for <see cref="IDisposable.Dispose" />.
      /// </summary>
      void IDisposable.Dispose()
      {
         var firstNotifyCollection = _firstCollection as INotifyCollectionChanged;
         if(firstNotifyCollection != null)
         {
            firstNotifyCollection.CollectionChanged -= onFirstCollectionChanged;
         }

         var secondNotifyCollection = _secondCollection as INotifyCollectionChanged;
         if(secondNotifyCollection != null)
         {
            secondNotifyCollection.CollectionChanged -= onSecondCollectionChanged;
         }

         _removedItemsCache = new List<TElement>();

         var firstCollection = _firstCollection.ToList();
         for(int i = firstCollection.Count - 1; i >= 0; i--)
         {
            _firstCollectionCount--;
            _removedItemsCache.Add(firstCollection[i]);

            EventsHelper.SafeInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, firstCollection[i], i));
         }

         var secondCollection = _secondCollection.ToList();
         for(int i = secondCollection.Count - 1; i >= 0; i--)
         {
            _secondCollectionCount--;

            _removedItemsCache.Add(secondCollection[i]);

            EventsHelper.SafeInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, secondCollection[i], i));
         }

         if(_autoDisposeFirstCollection)
         {
            _firstCollection.DisposeObject();
         }
         _firstCollection = null;

         if(_autoDisposeSecondCollection)
         {
            _secondCollection.DisposeObject();
         }
         _secondCollection = null;

         _removedItemsCache = null;
         EventsHelper.SafeInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

         _disposedGuard.Dispose();
      }

      #endregion

      #region IEnumerable<TElement> members

      /// <summary>
      ///    Implements for <see cref="IEnumerable{TElement}.GetEnumerator" />.
      /// </summary>
      IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
      {
         _disposedGuard.Check();
         if(_firstCollection != null)
         {
            foreach (var element in _firstCollection)
            {
               if(_removedItemsCache != null && _removedItemsCache.Contains(element))
               {
                  continue;
               }

               yield return element;
            }
         }

         if(_secondCollection != null)
         {
            foreach (var element in _secondCollection)
            {
               if(_removedItemsCache != null && _removedItemsCache.Contains(element))
               {
                  continue;
               }

               yield return element;
            }
         }
      }

      /// <summary>
      ///    Implements for <see cref="IEnumerable.GetEnumerator" />.
      /// </summary>
      IEnumerator IEnumerable.GetEnumerator()
      {
         _disposedGuard.Check();
         return ((IEnumerable<TElement>)this).GetEnumerator();
      }

      #endregion

      #region INotifyCollectionChanged members

      /// <summary>
      ///    Implements for <see cref="INotifyCollectionChanged.CollectionChanged" />.
      /// </summary>
      public event NotifyCollectionChangedEventHandler CollectionChanged;

      #endregion
   }
}
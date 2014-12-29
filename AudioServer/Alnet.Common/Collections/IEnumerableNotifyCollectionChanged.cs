#region Copyright

// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using CCTV.Framework.Utility;

namespace Alnet.Common.Collections
{
   /// <summary>
   ///    Collection <see cref="ICollection{TEntity}"/> that could raise notify about it changes.
   /// </summary>
   /// <typeparam name="TEntity">Type of Entity.</typeparam>
   public interface IEnumerableNotifyCollectionChanged<TEntity> : INotifyCollectionChanged, ICollection<TEntity>, INotifyPropertyChanged
      where TEntity : class
   {
      /// <summary>
      /// First element.
      /// </summary>
      TEntity First { get; }

      /// <summary>
      /// Last element.
      /// </summary>
      TEntity Last { get; }
   }

   /// <summary>
   /// Notifies about bulk operation with collection.
   /// </summary>
   public interface INotifyCollectionChangesCompleted
   {
      /// <summary>
      /// Notifies about bulk operation with collection.
      /// </summary>
      event EventHandler<DataEventArgs<NotifyCollectionChangedEventArgs[]>> CollectionChangesCompleted;

      /// <summary>
      /// Begin bulk modifications.
      /// </summary>
      void BeginBulkWrite();

      /// <summary>
      /// End bulk modifications.
      /// </summary>
      void EndBulkWrite();
   }
}

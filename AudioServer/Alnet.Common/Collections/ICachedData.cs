#region Copyright

// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using CCTV.Framework.Utility;

namespace Alnet.Common.Collections
{
   /// <summary>
   /// Interface for <see cref="CachedData{TKey,TItem}"/>.
   /// </summary>
   public interface ICachedData
   {
      /// <summary>
      /// Notifies about data changes.
      /// </summary>
      event EventHandler<DataEventArgs<string>> DataChanged;

      /// <summary>
      /// Load data and if loaded data differs from stored one - save it and notify about changes.
      /// </summary>
      /// <returns>
      /// <see langword="true"/> if data were changed.
      /// </returns>
      bool Load();

      /// <summary>
      /// Value indicating whether data were loaded into cache.
      /// </summary>
      bool IsLoaded { get; }

      /// <summary>
      /// Rises event.
      /// </summary>
      void RiseEvent();
   }
}

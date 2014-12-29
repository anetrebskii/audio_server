#region Copyright
// (c) 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.
#endregion

using System;
using System.Diagnostics;
using CCTV.Framework.Utility;

namespace Alnet.Common
{
   /// <summary>
   /// Implementation of common functionality for checking "object disposed" state.
   /// Shall be used in all <see cref="IDisposable"/> classes to controll access and double dispose.
   /// </summary>
   public sealed class DisposedGuard : IDisposable
   {

      #region Private Fields

      /// <summary>
      /// Value indicating whether object is disposed.
      /// </summary>
      private volatile bool _isDisposed;

      /// <summary>
      /// NAme of the owner object.
      /// </summary>
      private readonly string _ownerName;

      #endregion

      #region Constructor

      /// <summary>
      /// Initializes a new instance of the <see cref="DisposedGuard"/> object.
      /// </summary>
      /// <param name="ownerType">Type of the owner.</param>
      public DisposedGuard(Type ownerType)
      {
         Guard.VerifyArgumentNotNull(ownerType, "ownerType");

         _ownerName = ownerType.Name;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DisposedGuard"/> object.
      /// </summary>
      /// <param name="ownerName">Name of the owner.</param>
      public DisposedGuard(string ownerName)
      {
         _ownerName = Guard.EnsureArgumentNotNullOrEmpty(ownerName, "ownerName");
      }

      /// <summary>
      /// Creates Dispose guard for the <typeparamref name="T"/> type.
      /// </summary>
      /// <typeparam name="T">Type of guarded object.</typeparam>
      /// <returns>Dispose guard.</returns>
      public static DisposedGuard Create<T>()
      {
         return new DisposedGuard(typeof(T));
      }

      #endregion

      #region Public members

      /// <summary>
      /// Checks that object is not disposed.
      /// </summary>
      /// <exception cref="ObjectDisposedException"> in case of object is Disposed.</exception>
      public void Check()
      {
         if (_isDisposed)
         {
            throw new ObjectDisposedException(_ownerName);
         }
      }

      /// <summary>
      /// Gets value indicating whether object is disposed.
      /// </summary>
      public bool IsDisposed
      {
         [DebuggerStepThrough]
         get { return _isDisposed; }
      }

      #endregion

      #region Implementation of IDisposable

      /// <summary>
      /// Implements <see cref="IDisposable.Dispose"/>.
      /// </summary>
      public void Dispose()
      {
         if (_isDisposed)
         {
            throw new ObjectDisposedException(String.Format("Object: {0} already disposed.", _ownerName));
         }
         _isDisposed = true;
      }

      #endregion
   }
}

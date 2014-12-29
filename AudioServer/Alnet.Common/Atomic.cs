using System.Diagnostics;
using System.Security;

namespace Alnet.Common
{
   /// <summary> 
   /// Provides Atomic access to value types.
   /// </summary> 
   /// <typeparam name="T">Type of value.</typeparam>
   /// <example>
   /// <![CDATA[
   /// private readonly AtomicLock<UInt64> _atomic = new AtomicLock<ulong>(0);
   /// _atomic.Value = Helper.Generate(threadNumber); 
   /// var a = _atomic.Value; 
   /// if (!Helper.Verify(a)) 
   /// { 
   ///    Console.Write("F"); 
   /// } 
   /// ]]>
   /// </example>
   /// <remarks>
   /// <p>
   /// Reference access like: object referenceValue;<br/>
   /// referenceValue = newValue; - does not requires locking - its atomic by itself.
   /// </p>
   /// </remarks>
   [SecurityCritical]
   [DebuggerStepThrough]
   public sealed class Atomic<T> where T:struct
   {
      /// <summary>
      /// Boxed reference to Value Type.
      /// </summary>
      /// <remarks>
      /// Due to atomic reference set operations it is not needed to synchronize setting or getting boxed reference type.
      /// </remarks>
      private object _boxedObject;

      /// <summary>
      /// Initializes a new instance of the <see cref="Atomic{T}"/> class.
      /// </summary>
      public Atomic(): this(default(T))
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Atomic{T}"/> class.
      /// </summary>
      /// <param name="value">Initial value.</param>
      public Atomic(T value)
      {
         _boxedObject = value;
      }

      /// <summary>
      /// Atomic access.
      /// </summary>
      public T Value
      {
         get
         {
            // Makes a copy of current data type.
            return (T)_boxedObject;
         }
         set { _boxedObject = value; }
      }
   } 
}

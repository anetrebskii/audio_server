using System;
using System.Diagnostics;

namespace Alnet.Common
{
   /// <summary>
   /// This class represents Event Argument with one data parapeter passed.
   /// </summary>
   /// <typeparam name="T">Type of the data.</typeparam>
   [DebuggerStepThrough]
   public sealed class DataEventArgs<T> : EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="DataEventArgs{T}"/> class.
      /// </summary>
      /// <param name="data">Data parameter.</param>
      public DataEventArgs(T data)
      {
         Data = data;
      }

      /// <summary>
      /// Gets the data parameter.
      /// </summary>
      public T Data { get; private set; }
   }

   /// <summary>
   /// This class represents Event Argument with two data parapeters passed.
   /// </summary>
   /// <typeparam name="T1">Type of the first parameter.</typeparam>
   /// <typeparam name="T2">Type of the second parameter.</typeparam>
   [DebuggerStepThrough]
   public sealed class DataEventArgs<T1, T2> : EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="DataEventArgs{T}"/> class.
      /// </summary>
      /// <param name="data1">First data parameter.</param>
      /// <param name="data2">Second data parameter.</param>
      public DataEventArgs(T1 data1, T2 data2)
      {
         Data1 = data1;
         Data2 = data2;
      }

      /// <summary>
      /// Gets the first data parameter.
      /// </summary>
      public T1 Data1 { get; private set; }

      /// <summary>
      /// Gets the second data parameter.
      /// </summary>
      public T2 Data2 { get; private set; }
   }

   /// <summary>
   /// This class represents Event Argument with two data parapeters passed.
   /// </summary>
   /// <typeparam name="T1">Type of the first parameter.</typeparam>
   /// <typeparam name="T2">Type of the second parameter.</typeparam>
   /// <typeparam name="T3">Type of the third parameter.</typeparam>
   [DebuggerStepThrough]
   public sealed class DataEventArgs<T1, T2, T3> : EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="DataEventArgs{T}"/> class.
      /// </summary>
      /// <param name="data1">First data parameter.</param>
      /// <param name="data2">Second data parameter.</param>
      /// <param name="data3">Third data parameter.</param>
      public DataEventArgs(T1 data1, T2 data2, T3 data3)
      {
         Data1 = data1;
         Data2 = data2;
         Data3 = data3;
      }

      /// <summary>
      /// Gets the first data parameter.
      /// </summary>
      public T1 Data1 { get; private set; }

      /// <summary>
      /// Gets the second data parameter.
      /// </summary>
      public T2 Data2 { get; private set; }

      /// <summary>
      /// Gets the third data parameter.
      /// </summary>
      public T3 Data3 { get; private set; }
   }
}
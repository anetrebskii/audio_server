using System;
using System.IO;

namespace Alnet.Common
{
   /// <summary>
   /// Helper routines to work with <see cref="MemoryStream"/>
   /// </summary>
   public static class MemoryStreamHelper
   {
      /// <summary>
      /// Returns a copy of a data really stored in <paramref name="stream"/>.
      /// </summary>
      /// <param name="stream">Stream to get data from.</param>
      /// <returns>Copied data.</returns>
      public static byte[] GetData(this MemoryStream stream)
      {
         if (stream == null)
         {
            return null;
         }

         var data = new byte[stream.Length];

         Array.Copy(stream.GetBuffer(), data, stream.Length);

         return data;
      }
   }
}

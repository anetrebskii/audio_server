using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio.Wave.WaveOutputs
{
   class StreamBufferManager
   {
      private byte[][] _buffers;

      public StreamBufferManager(int numberBuffers, int bufferSize)
      {
         _buffers = new byte[numberBuffers][];
         for (int i = 0; i < numberBuffers; i++)
         {
            _buffers[i] = new byte[bufferSize];
         }
      }

      public void Write(int bufferIndex, byte[] buffer)
      {
         if (_buffers[bufferIndex].Length != buffer.Length)
         {
            throw new ArgumentException("buffer has length not equals to internal buffer");
         }
         buffer.CopyTo(_buffers[bufferIndex], 0);
      }


      public byte[] GetBuffer(int index)
      {
         return _buffers[index];
      }
   }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio.Wave.WaveOutputs
{
   class WaveProviderReader
   {
      private byte[] _buffer;
      private IWaveProvider _waveProvider;

      public WaveProviderReader(int bufferSize, IWaveProvider waveProvider)
      {
         _buffer = new byte[bufferSize];
         _waveProvider = waveProvider;
      }

      public int BufferSize
      {
         get { return _buffer.Length; }
      }

      public IWaveProvider WaveProvider
      {
         get { return _waveProvider; }
      }

      public bool Read(out byte[] buffer)
      {
         
         int readCount;
         lock (_waveProvider)
         {
            readCount = _waveProvider.Read(_buffer, 0, _buffer.Length);
         }

         //buffer = new byte[readCount];
         buffer = _buffer;
         if (readCount > 0)
         {
            
            for (int i = readCount; i < _buffer.Length; i++)
            {
               _buffer[i] = 0;
            }
            //_buffer.CopyTo(buffer, 0);
            return true;
         }
         return false;
      }
   }
}

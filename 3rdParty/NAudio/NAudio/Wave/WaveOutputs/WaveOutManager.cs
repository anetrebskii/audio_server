using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAudio.Wave.WaveOutputs
{
   class WaveOutManager : IDisposable
   {
      private readonly WaveOutBuffer[] _buffers;
      private readonly IntPtr hWaveOut;
      private readonly object waveOutLock = new object();
      readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
      private int _deviceNumber;
      

      public WaveOutManager(StreamBufferManager bufferManager, int deviceNumber, int numberBuffers, WaveProviderReader waveStreamReader)
      {
         MmResult result;
         _deviceNumber = deviceNumber;

         result = WaveInterop.waveOutOpenWindow(out hWaveOut, (IntPtr)deviceNumber, waveStreamReader.WaveProvider.WaveFormat, _waitHandle.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
         MmException.Try(result, "waveOutOpen");
         
         _buffers = new WaveOutBuffer[numberBuffers];
         for (int n = 0; n < numberBuffers; n++)
         {
            _buffers[n] = new WaveOutBuffer(hWaveOut, bufferManager.GetBuffer(n), waveOutLock);
         }
      }

      public void UpdateReadState()
      {
         _waitHandle.Set();
      }
      
      public int DeviceNumber
      {
         get { return _deviceNumber; }
      }

      public WaitHandle WaitHandle
      {
         get { return _waitHandle; }
      }

      public bool SendToDevice(int bufferIndex)
      {
         _buffers[bufferIndex].OnDone();
         return true;
      }

      public bool IsInQueue(int bufferIndex)
      {
         return _buffers[bufferIndex].InQueue;
      }

      public void Pause()
      {
         MmResult result;
         lock (waveOutLock)
         {
            result = WaveInterop.waveOutPause(hWaveOut);
         }
         if (result != MmResult.NoError)
         {
            throw new MmException(result, "waveOutPause");
         }
      }

      public void Resume()
      {
         MmResult result;
         lock (waveOutLock)
         {
            result = WaveInterop.waveOutRestart(hWaveOut);
         }
         if (result != MmResult.NoError)
         {
            throw new MmException(result, "waveOutRestart");
         }
      }

      public void Stop()
      {
         MmResult result;
         lock (waveOutLock)
         {
            result = WaveInterop.waveOutReset(hWaveOut);
         }
         if (result != MmResult.NoError)
         {
            throw new MmException(result, "waveOutReset");
         }
      }

      public long GetPosition()
      {
         lock (waveOutLock)
         {
            MmTime mmTime = new MmTime();
            mmTime.wType = MmTime.TIME_BYTES; // request results in bytes, TODO: perhaps make this a little more flexible and support the other types?
            MmException.Try(WaveInterop.waveOutGetPosition(hWaveOut, out mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");

            if (mmTime.wType != MmTime.TIME_BYTES)
               throw new Exception(string.Format("waveOutGetPosition: wType -> Expected {0}, Received {1}", MmTime.TIME_BYTES, mmTime.wType));

            return mmTime.cb;
         }
      }

      public void Dispose()
      {
         foreach (var buffer in _buffers)
         {
            buffer.Dispose();
         }
         lock (waveOutLock)
         {
            WaveInterop.waveOutClose(hWaveOut);
         }
      }
   }
}

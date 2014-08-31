using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using NAudio.Wave.WaveOutputs;

namespace NAudio.Wave
{
    /// <summary>
    /// Alternative WaveOut class, making use of the Event callback
    /// </summary>
    public class WaveOutEvent : IWavePlayer, IWavePosition
    {
        private readonly object waveOutLock;
        private readonly SynchronizationContext syncContext;
        //private IntPtr hWaveOut; // WaveOut handle
        //private WaveOutBuffer[] buffers;
        private WaveProviderReader waveStreamReader;
        private volatile PlaybackState playbackState;

        /// <summary>
        /// Indicates playback has stopped automatically
        /// </summary>
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        /// <summary>
        /// Gets or sets the desired latency in milliseconds
        /// Should be set before a call to Init
        /// </summary>
        public int DesiredLatency { get; set; }

        /// <summary>
        /// Gets or sets the number of buffers used
        /// Should be set before a call to Init
        /// </summary>
        public int NumberOfBuffers { get; set; }

        /// <summary>
        /// Gets or sets the device number
        /// Should be set before a call to Init
        /// This must be between 0 and <see>DeviceCount</see> - 1.
        /// </summary>
        public int DeviceNumber { get; set; }

        /// <summary>
        /// Opens a WaveOut device
        /// </summary>
        public WaveOutEvent()
        {
            this.syncContext = SynchronizationContext.Current;
            // set default values up
            this.DeviceNumber = 0;
            this.DesiredLatency = 300;
            this.NumberOfBuffers = 2;

            this.waveOutLock = new object();
        }

       private StreamBufferManager _bufferManager;
       private readonly List<WaveOutManager> _waveOutManagers = new List<WaveOutManager>();

       /// <summary>
       /// Initialises the WaveOut device
       /// </summary>
       /// <param name="waveProvider">WaveProvider to play</param>
       public void Init(IWaveProvider waveProvider)
       {
          if (playbackState != PlaybackState.Stopped)
          {
             throw new InvalidOperationException("Can't re-initialize during playback");
          }
          // normally we don't allow calling Init twice, but as experiment, see if we can clean up and go again
          // try to allow reuse of this waveOut device
          // n.b. risky if Playback thread has not exited
          DisposeBuffers();

          int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1)/NumberOfBuffers);
          _bufferManager = new StreamBufferManager(NumberOfBuffers, bufferSize);


          this.waveStreamReader = new WaveProviderReader(bufferSize, waveProvider);
          foreach (var waveOutManager in _waveOutManagers)
          {
             waveOutManager.Dispose();
          }
          _waveOutManagers.Clear();
       }

       public void AddWaveoutManager(int deviceNumber)
       {          
          _waveOutManagers.Add(new WaveOutManager(_bufferManager, deviceNumber, NumberOfBuffers, waveStreamReader));
       }

       public void RemoveWaveoutManager(int deviceNumber)
       {
          _waveOutManagers.Remove(_waveOutManagers.FirstOrDefault(s => s.DeviceNumber == deviceNumber));
       }

        /// <summary>
        /// Start playing the audio from the WaveStream
        /// </summary>
        public void Play()
        {
            if (this._waveOutManagers == null || this.waveStreamReader == null)
            {
                throw new InvalidOperationException("Must call Init first");
            }
            if (playbackState == PlaybackState.Stopped)
            {
                playbackState = PlaybackState.Playing;
                ThreadPool.QueueUserWorkItem((state) => PlaybackThread(), null);
            }
            else if (playbackState == PlaybackState.Paused)
            {               
                Resume();
               _waveOutManagers.ForEach(w => w.UpdateReadState());
            }
        }

        private void PlaybackThread()
        {
            Exception exception = null;
            try
            {
                DoPlayback();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                playbackState = PlaybackState.Stopped;
                // we're exiting our background thread
                RaisePlaybackStoppedEvent(exception);
            }
        }

        private void DoPlayback()
        {
            WaitHandle[] waitHandles = _waveOutManagers.Select(wout => wout.WaitHandle).ToArray();
            int count = _waveOutManagers.Count;
            while (playbackState != PlaybackState.Stopped)
            {
                if (count != _waveOutManagers.Count)
                {
                    waitHandles = _waveOutManagers.Select(wout => wout.WaitHandle).ToArray();
                }
                if (WaitHandle.WaitAny(waitHandles) == 0)
                {
                    byte[] streamBuffer = new byte[0];
                    // requeue any buffers returned to us
                    if (playbackState == PlaybackState.Playing)
                    {

                        bool existsDataToPlay = false;
                        for (int i = 0; i < NumberOfBuffers; i++)
                        {
                            if (_waveOutManagers.All(m => !m.IsInQueue(i)))
                            {
                                if (waveStreamReader.Read(out streamBuffer))
                                {
                                    _bufferManager.Write(i, streamBuffer);
                                    _waveOutManagers.ForEach(m => m.SendToDevice(i));
                                    existsDataToPlay = true;
                                }
                            }
                            else
                            {
                                existsDataToPlay = true;
                            }
                        }

                        if (!existsDataToPlay)
                        {
                            // we got to the end
                            this.playbackState = PlaybackState.Stopped;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public void Pause()
        {
            if (playbackState == PlaybackState.Playing)
            {
               _waveOutManagers.ForEach(w => w.Pause());
                playbackState = PlaybackState.Paused;
            }
        }

        /// <summary>
        /// Resume playing after a pause from the same position
        /// </summary>
        private void Resume()
        {
            if (playbackState == PlaybackState.Paused)
            {
               _waveOutManagers.ForEach(w => w.Resume());
                playbackState = PlaybackState.Playing;
            }
        }

        /// <summary>
        /// Stop and reset the WaveOut device
        /// </summary>
        public void Stop()
        {
            if (playbackState != PlaybackState.Stopped)
            {
                // in the call to waveOutReset with function callbacks
                // some drivers will block here until OnDone is called
                // for every buffer
                playbackState = PlaybackState.Stopped; // set this here to avoid a problem with some drivers whereby 

                _waveOutManagers.ForEach(w =>
                   {
                      w.Stop();
                      w.UpdateReadState();
                   }); // give the thread a kick, make sure we exit
            }
        }

        /// <summary>
        /// Gets the current position in bytes from the wave output device.
        /// (n.b. this is not the same thing as the position within your reader
        /// stream - it calls directly into waveOutGetPosition)
        /// </summary>
        /// <returns>Position in bytes</returns>
        public long GetPosition()
        {
           return _waveOutManagers.First().GetPosition();
        }

        /// <summary>
        /// Gets a <see cref="Wave.WaveFormat"/> instance indicating the format the hardware is using.
        /// </summary>
        public WaveFormat OutputWaveFormat
        {
            get { return this.waveStreamReader.WaveProvider.WaveFormat; }
        }

        /// <summary>
        /// Playback State
        /// </summary>
        public PlaybackState PlaybackState
        {
            get { return playbackState; }
        }

        /// <summary>
        /// Obsolete property
        /// </summary>
        [Obsolete]
        public float Volume
        {
            get { return 1.0f; }
            set { if (value != 1.0f) throw new NotImplementedException(); }
        }

        #region Dispose Pattern

        /// <summary>
        /// Closes this WaveOut device
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Closes the WaveOut device and disposes of buffers
        /// </summary>
        /// <param name="disposing">True if called from <see>Dispose</see></param>
        protected void Dispose(bool disposing)
        {
            Stop();

            if (disposing)
            {
                DisposeBuffers();
            }
        }

        private void DisposeBuffers()
        {
            if (_waveOutManagers != null)
            {
               foreach (var waveOutManager in _waveOutManagers)
               {
                  waveOutManager.Dispose();   
               }
               
               _waveOutManagers.Clear();
            }
        }

        /// <summary>
        /// Finalizer. Only called when user forgets to call <see>Dispose</see>
        /// </summary>
        ~WaveOutEvent()
        {
            System.Diagnostics.Debug.Assert(false, "WaveOutEvent device was not closed");
            Dispose(false);
        }

        #endregion

        private void RaisePlaybackStoppedEvent(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (this.syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    this.syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }
    }
}

using System;
using System.IO;
using NAudio.Wave;

namespace Alnet.AudioServer.Components.AudioPlayer
{
    class WaveSound : IDisposable
    {
        private WaveOutEvent _waveOut = new WaveOutEvent();
        private MediaFoundationReader _mediaFoundationReader;

        public WaveSound(SoundInfo soundInfo)
        {
            _waveOut.NumberOfBuffers = 2;
            _waveOut.DesiredLatency = 100;
            _mediaFoundationReader = new MediaFoundationReader(soundInfo.Url);
            _waveOut.Init(_mediaFoundationReader);            
            _waveOut.PlaybackStopped += waveOutOnPlaybackStopped;             
        }

        private void waveOutOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            EventHandler hanler = Completed;
            if (hanler != null)
            {
                hanler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Completed;

        public long CurrentPosition
        {
            get { return _mediaFoundationReader.Position; }
        }

        public void Play()
        {
            _waveOut.Play();
        }

        public void Stop()
        {
            _waveOut.Stop();
        }

        public void Dispose()
        {           
            _waveOut.PlaybackStopped -= waveOutOnPlaybackStopped;
            _waveOut.Dispose();
            _mediaFoundationReader.Dispose();
        }

        public void EnableSoundCard(int index)
        {
            _waveOut.AddWaveoutManager(index);
        }

        public void DisableSoundCard(int index)
        {
            _waveOut.RemoveWaveoutManager(index);
        }
    }
}

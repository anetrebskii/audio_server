using System;
using System.IO;
using NAudio.Wave;

namespace Alnet.AudioServer.Components.AudioPlayer
{
    class Sound : IDisposable
    {
        private WaveOutEvent _waveOut = new WaveOutEvent();
        private Mp3FileReader _mp3FileReader;

        public Sound(byte[] soundData)
        {
            _waveOut.NumberOfBuffers = 2;
            _waveOut.DesiredLatency = 100;
            _mp3FileReader = new Mp3FileReader(new MemoryStream(soundData));
            _waveOut.Init(_mp3FileReader);            
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
            get { return _mp3FileReader.Position; }
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

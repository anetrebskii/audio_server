using System;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.Common;
using NAudio.Wave;

namespace Alnet.AudioServer.Components.NAudioServer
{
    /// <summary>
    /// Playing sound in the host.
    /// </summary>
    internal sealed class WaveSound : IDisposable
    {
        #region Private fields

        /// <summary>
        /// Provides a playing audio on the host.
        /// </summary>
        private readonly WaveOutEvent _waveOut;

        /// <summary>
        /// The reader for media content.
        /// </summary>
        private readonly MediaFoundationReader _mediaFoundationReader;

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(WaveSound));

        /// <summary>
        /// Information about the sound.
        /// </summary>
        private readonly SoundInfo _soundInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveSound"/> class.
        /// </summary>
        /// <param name="soundInfo">The sound information.</param>
        public WaveSound(SoundInfo soundInfo)
        {
            _soundInfo = Guard.EnsureArgumentNotNull(soundInfo, "soundInfo");
            _mediaFoundationReader = new MediaFoundationReader(soundInfo.Url);
            _waveOut = new WaveOutEvent
            {
                NumberOfBuffers = 2,
                DesiredLatency = 100
            };
            _waveOut.Init(_mediaFoundationReader);
            _waveOut.PlaybackStopped += waveOutOnPlaybackStopped;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Information about the sound.
        /// </summary>
        public SoundInfo SoundInfo
        {
            get { return _soundInfo; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Occurs when playback stopped.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Current position of the playback in the bytes.
        /// </summary>
        public long CurrentPosition
        {
            get { return _mediaFoundationReader.Position; }
            set { _mediaFoundationReader.Position = value; }
        }

        /// <summary>
        /// Length of the sound in the bytes.
        /// </summary>
        public long Length
        {
            get { return _mediaFoundationReader.Length; }
        }

        /// <summary>
        /// Starts playing the sound.
        /// </summary>
        public void Play()
        {
            _disposedGuard.Check();
            _waveOut.Play();
        }

        /// <summary>
        /// Stops playing the sound.
        /// </summary>
        public void Stop()
        {
            _disposedGuard.Check();
            _waveOut.Stop();
        }

        /// <summary>
        /// Enables the sound card.
        /// </summary>
        /// <param name="index">The index of the sound card.</param>
        public void EnableSoundCard(int index)
        {
            _disposedGuard.Check();
            _waveOut.AddWaveoutManager(index);
        }

        /// <summary>
        /// Disables the sound card.
        /// </summary>
        /// <param name="index">The index of the sound card.</param>
        public void DisableSoundCard(int index)
        {
            _disposedGuard.Check();
            _waveOut.RemoveWaveoutManager(index);
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _disposedGuard.Dispose();
            _waveOut.PlaybackStopped -= waveOutOnPlaybackStopped;
            _waveOut.Dispose();
            _mediaFoundationReader.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Waves the out on playback stopped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="stoppedEventArgs">The <see cref="StoppedEventArgs"/> instance containing the event data.</param>
        private void waveOutOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            if (_disposedGuard.IsDisposed)
            {
                return;
            }
            EventsHelper.InvokeEventHandler(Completed, this, EventArgs.Empty);
        }

        #endregion
    }
}

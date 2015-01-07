using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Activation;

using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.Common;

namespace Alnet.AudioServer.Components.NAudioServer
{
    internal sealed class PlaylistAudioPlayer : IPlaylistAudioPlayer, IDisposable
    {
        #region Private fields

        /// <summary>
        /// Used audio cards.
        /// </summary>
        private readonly HashSet<int> _soundCards = new HashSet<int>();

        /// <summary>
        /// Provide a sound playlist
        /// </summary>
        private readonly IPlaylistSoundProvider _playlistSoundProvider = null;

        /// <summary>
        /// Index of the current sound in the playlist.
        /// </summary>
        private int _currentSoundIndex;

        /// <summary>
        /// The instance of the current sound in the playlist.
        /// </summary>
        private WaveSound _currentWaveSound;

        /// <summary>
        /// The cached sound list from the <see cref="_playlistSoundProvider"/>
        /// </summary>
        private SoundInfo[] _cachedSoundList;

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(PlaylistAudioPlayer));

        #endregion

        #region Constructors

        public PlaylistAudioPlayer(IPlaylistSoundProvider playlistSoundProvider)
        {
            _playlistSoundProvider = Guard.EnsureArgumentNotNull(playlistSoundProvider, "playlistSoundProvider");
            _playlistSoundProvider.SoundsChanged += playlistSoundProviderOnPlaylistSoundsChanged;
            _cachedSoundList = _playlistSoundProvider.GetSounds();
            if (_cachedSoundList.Length > 0)
            {
                actualizeCurrentWaveSound(_currentSoundIndex);
            }
        }

        #endregion

        #region IPlaylistAudioPlayer members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Play()
        {
            _disposedGuard.Check();
            verifyAvailableSoundCards();
            actualizeCurrentWaveSound(_currentSoundIndex);
            _currentWaveSound.Play();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            _disposedGuard.Check();
            if (_currentWaveSound == null)
            {
                return;
            }
            _currentWaveSound.Stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EnableChannel(int index)
        {
            if (_soundCards.Contains(index))
            {
                return;
            }
            _soundCards.Add(index);
            if (_currentWaveSound != null)
            {
                _currentWaveSound.EnableSoundCard(index);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DisableChannel(int index)
        {
            if (!_soundCards.Contains(index))
            {
                return;
            }
            _soundCards.Remove(index);
            if (_currentWaveSound != null)
            {
                if (_soundCards.Count == 0)
                {
                    // Wave sound doesn't exists without channels.
                    disposeWaveSound(_currentWaveSound);
                    _currentWaveSound = null;
                }
                else
                {
                    _currentWaveSound.DisableSoundCard(index);   
                }
            }            
        }

        public int[] GetEnabledChannels()
        {
            return _soundCards.ToArray();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Play(int soundIndex)
        {
            verifyAvailableSoundCards();
            actualizeCurrentWaveSound(soundIndex);
            _currentWaveSound.Play();
        }

        public int CurrentSoundIndex
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _currentSoundIndex; }
        }

        public SoundInfo CurrentSound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_currentWaveSound == null)
                {
                    return null;
                }
                return _currentWaveSound.SoundInfo;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SoundInfo[] GetSounds()
        {
            return _cachedSoundList.ToArray();
        }

        public double PlaybackPosition
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _currentWaveSound == null ? 0 : (double)_currentWaveSound.CurrentPosition / _currentWaveSound.Length;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                Guard.VerifyArgumentInRange(value, 0, 1, "Playback position must be in bounds [0..1]");
                if (_currentWaveSound == null)
                {
                    return;
                }
                _currentWaveSound.CurrentPosition = (long)(value * _currentWaveSound.Length);
            }
        }

        #endregion

        #region IDisposable members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            _playlistSoundProvider.SoundsChanged -= playlistSoundProviderOnPlaylistSoundsChanged;
            if (_currentWaveSound != null)
            {
                disposeWaveSound(_currentWaveSound);
                _currentWaveSound = null;
            }
        }

        #endregion

        #region Private methods

        private void verifyAvailableSoundCards()
        {
            if (_soundCards.Count == 0)
            {
                throw new PlayerException(PlayerExceptionTypes.NoSoundCards);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void playlistSoundProviderOnPlaylistSoundsChanged(object sender, EventArgs eventArgs)
        {
            _cachedSoundList = _playlistSoundProvider.GetSounds();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void currentWaveSoundOnCompleted(object sender, EventArgs eventArgs)
        {
            if (_disposedGuard.IsDisposed)
            {
                return;
            }
            try
            {
                actualizeCurrentWaveSound(_currentSoundIndex + 1);
                _currentWaveSound.Play();
            }
            catch (PlayerException ex)
            {
                if (ex.Type != PlayerExceptionTypes.NoSounds)
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Actualizes the <see cref="_currentWaveSound"/> and <see cref="_currentSoundIndex"/> with <paramref cref="soundIndex"/>.
        /// </summary>
        /// <param name="soundIndex">Index of the sound.</param>
        private void actualizeCurrentWaveSound(int soundIndex)
        {
            if (_cachedSoundList.Length == 0)
            {
                throw new PlayerException(PlayerExceptionTypes.NoSounds);
            }
            int correctedSoundIndex = correctSoundIndex(soundIndex);
            bool needToInitializeNewSound = false;
            if (_currentSoundIndex == correctedSoundIndex)
            {
                if (_currentWaveSound == null)
                {
                    needToInitializeNewSound = true;
                }
            }
            else
            {
                _currentSoundIndex = correctedSoundIndex;
                if (_currentWaveSound != null)
                {
                    disposeWaveSound(_currentWaveSound);
                    _currentWaveSound = null;
                }
                needToInitializeNewSound = true;
            }
            if (needToInitializeNewSound)
            {
                _currentWaveSound = initializeWaveSound(_cachedSoundList[correctedSoundIndex]);
            }
        }

        private void disposeWaveSound(WaveSound waveSound)
        {
            waveSound.Completed -= currentWaveSoundOnCompleted;
            waveSound.Dispose();
        }

        private int correctSoundIndex(int soundIndex)
        {
            if (soundIndex < 0)
            {
                return _cachedSoundList.Length - 1;
            }
            if (soundIndex >= _cachedSoundList.Length)
            {
                return 0;
            }
            return soundIndex;
        }

        private WaveSound initializeWaveSound(SoundInfo soundInfo)
        {
            WaveSound returnValue = new WaveSound(soundInfo);
            foreach (var soundCard in _soundCards)
            {
                returnValue.EnableSoundCard(soundCard);
            }
            returnValue.Completed += currentWaveSoundOnCompleted;
            return returnValue;
        }

        #endregion
    }
}

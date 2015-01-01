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
        /// 
        /// <remarks>
        /// Get access to it's items via the method <see cref="getSoundInfoFromPlaylist"/>
        /// </remarks>
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
            _soundCards.Add(index);
            if (_currentWaveSound != null)
            {
                _currentWaveSound.EnableSoundCard(index);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DisableChannel(int index)
        {
            _soundCards.Remove(index);
            if (_currentWaveSound != null)
            {
                _currentWaveSound.DisableSoundCard(index);
            }
            verifyAvailableSoundCards();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Play(int soundIndex)
        {
            verifyAvailableSoundCards();
            actualizeCurrentWaveSound(soundIndex);
            _currentWaveSound.Play();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetCurrentSoundIndex()
        {
            return _currentSoundIndex;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SoundInfo[] GetSounds()
        {
            return _cachedSoundList.ToArray();
        }

        public long PlaybackPosition
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _currentWaveSound == null ? 0 : _currentWaveSound.CurrentPosition;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_currentWaveSound == null)
                {
                    return;
                }
                _currentWaveSound.CurrentPosition = value;
            }
        }

        public long CurrentSoundDuration
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _currentWaveSound == null ? 0 : _currentWaveSound.Length; }
        }

        #endregion

        #region IDisposable members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            _playlistSoundProvider.SoundsChanged -= playlistSoundProviderOnPlaylistSoundsChanged;
            if (_currentWaveSound != null)
            {
                _currentWaveSound.Completed -= currentWaveSoundOnCompleted;
                _currentWaveSound.Dispose();
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

        private SoundInfo getSoundInfoFromPlaylist(int soundIndex)
        {
            SoundInfo[] sounds = _cachedSoundList.ToArray();
            if (sounds.Length == 0)
            {
                throw new PlayerException(PlayerExceptionTypes.NoSounds);
            }
            return soundIndex < sounds.Length ? sounds[soundIndex] : sounds.Last();
        }

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
            int nextSoundIndex = _currentSoundIndex + 1;
            if (nextSoundIndex >= _cachedSoundList.Length)
            {
                nextSoundIndex = 0;
            }
            try
            {
                actualizeCurrentWaveSound(nextSoundIndex);
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
            if (_currentSoundIndex == soundIndex)
            {
                if (_currentWaveSound == null)
                {
                    _currentWaveSound = initializeWaveSound(getSoundInfoFromPlaylist(soundIndex));
                }
            }
            else
            {
                _currentSoundIndex = soundIndex;
                if (_currentWaveSound != null)
                {                    
                    _currentWaveSound.Completed -= currentWaveSoundOnCompleted;
                    _currentWaveSound.Dispose();
                }
                _currentWaveSound = initializeWaveSound(getSoundInfoFromPlaylist(soundIndex));
            }
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

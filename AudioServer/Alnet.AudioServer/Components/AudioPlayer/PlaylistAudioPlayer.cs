using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NAudio.Wave;

namespace Alnet.AudioServer.Components.AudioPlayer
{
   class PlaylistAudioPlayer : IPlaylistAudioPlayer, IDisposable
   {
      private readonly HashSet<int> _soundCards = new HashSet<int>();
      private readonly ISoundProvider _soundProvider = null;
      private int _currentSoundIndex;
      private WaveSound _currentWaveSound;
      private SoundInfo[] _cachedSoundList;       

      public PlaylistAudioPlayer(ISoundProvider soundProvider)
      {
         _soundProvider = soundProvider;
         _soundProvider.SoundListChanged += soundProviderOnSoundListChanged;
         _cachedSoundList = _soundProvider.GetSoundList();
      }

      #region IPlaylistAudioPlayer members

      public void Play()
      {
         if (_currentWaveSound == null)
         {
            if (_cachedSoundList.Length == 0)
            {
               throw new PlayerException("No sounds");
            }
            initializeCurrentSound();
         }
         _currentWaveSound.Play();
      }

      public void Stop()
      {
         if (_currentWaveSound == null)
         {
            return;
         }
         _currentWaveSound.Stop();
      }

      public ChannelInfo[] GetChannels()
      {
         List<ChannelInfo> returnValue = new List<ChannelInfo>();
         for (int i = 0; i < WaveOut.DeviceCount; i++)
         {
            returnValue.Add(new ChannelInfo()
            {
               Index = i,
               Description = WaveOut.GetCapabilities(i).ProductName
            });
         }
         return returnValue.ToArray();
      }

      public void EnableChannel(int index)
      {
         _soundCards.Add(index);
         if (_currentWaveSound != null)
         {
            _currentWaveSound.EnableSoundCard(index);
         }
      }

      public void DisableChannel(int index)
      {
         _soundCards.Remove(index);
         if (_currentWaveSound != null)
         {
            _currentWaveSound.DisableSoundCard(index);
         }
      }

      public void Play(int index)
      {
         loadSound(index);
         _currentWaveSound.Play();
      }

      public int GetCurrentSoundIndex()
      {
         return _currentSoundIndex;
      }

      public SoundInfo[] GetPlayList()
      {
         return _cachedSoundList;
      }

       public long CurrentPosition
       {
           get { return _currentWaveSound == null ? 0 : _currentWaveSound.CurrentPosition; }
           set
           {
               if (_currentWaveSound == null)
               {
                   return;
               }
               _currentWaveSound.CurrentPosition = value;
           }
       }

       public long Length
       {
           get { return _currentWaveSound == null ? 0 : _currentWaveSound.Length; }
       }

       #endregion

      #region IDisposable members

      public void Dispose()
      {
         _soundProvider.SoundListChanged -= soundProviderOnSoundListChanged;
         if (_currentWaveSound != null)
         {
            _currentWaveSound.Completed -= CurrentWaveSoundOnCompleted;
            _currentWaveSound.Dispose();
         }
      }

      #endregion

      #region Private methods

      private void soundProviderOnSoundListChanged(object sender, EventArgs eventArgs)
      {
         _cachedSoundList = _soundProvider.GetSoundList();
      }

      private void initializeCurrentSound()
      {
         _currentWaveSound = new WaveSound(_cachedSoundList[_currentSoundIndex]);
         foreach (var soundCard in _soundCards)
         {
            _currentWaveSound.EnableSoundCard(soundCard);            
         }         
         _currentWaveSound.Completed += CurrentWaveSoundOnCompleted;
      }

      private void CurrentWaveSoundOnCompleted(object sender, EventArgs eventArgs)
      {
         loadNextSound();
         _currentWaveSound.Play();
      }

      private void loadNextSound()
      {
         int nextSoundIndex = _currentSoundIndex + 1;
         if (nextSoundIndex >= _cachedSoundList.Length)
         {
            nextSoundIndex = 0;
         }
         loadSound(nextSoundIndex);
      }

      private void loadSound(int soundIndex)
      {
         _currentSoundIndex = soundIndex;
         _currentWaveSound.Completed -= CurrentWaveSoundOnCompleted;
         _currentWaveSound.Dispose();

         initializeCurrentSound();
      }

      #endregion
   }
}

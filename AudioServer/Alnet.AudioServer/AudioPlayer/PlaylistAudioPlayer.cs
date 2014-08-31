using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Alnet.AudioServer.AudioPlayer
{
    class PlaylistAudioPlayer : IAudioPlayer, IDisposable
    {
        private ISoundProvider _soundProvider = null;
        private int _currentSoundIndex;
        private Sound _currentSound;
        private SoundInfo[] _soundList;

        public PlaylistAudioPlayer(ISoundProvider soundProvider)
        {
            _soundProvider = soundProvider;
            _soundList = _soundProvider.GetSoundList();
        }

        public SoundInfo[] GetSounds()
        {
            return _soundList;
        }

        public void Play()
        {
            if (_currentSound == null)
            {
                _currentSound = new Sound(_soundProvider.GetSoundData(_currentSoundIndex));
                _currentSound.Completed += currentSoundOnCompleted;
            }
            _currentSound.Play();
        }

        private void currentSoundOnCompleted(object sender, EventArgs eventArgs)
        {
            loadNextSound();
        }

        private void loadNextSound()
        {
            _currentSoundIndex++;
            if (_currentSoundIndex >= _soundList.Length)
            {
                _currentSoundIndex = 0;
            }

            _currentSound.Completed -= currentSoundOnCompleted;
            _currentSound.Dispose();

            _currentSound = new Sound(_soundProvider.GetSoundData(_currentSoundIndex));
            _currentSound.Completed += currentSoundOnCompleted;
        }

        public void Stop()
        {
            if (_currentSound == null)
            {
                return;
            }
            _currentSound.Stop();
        }

        public void AddChannel(int index)
        {
            if (_currentSound == null)
            {
                _currentSound = new Sound(_soundProvider.GetSoundData(_currentSoundIndex));
                _currentSound.Completed += currentSoundOnCompleted;
            }
            _currentSound.AddChannel(index);
        }

        public void RemoveChannel(int index)
        {
            if (_currentSound == null)
            {
                return;
            }
            _currentSound.RemoveChannel(index);
        }

        public void Dispose()
        {
            if (_currentSound != null)
            {
                _currentSound.Dispose();
            }
        }
    }
}

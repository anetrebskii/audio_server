using System;

namespace Alnet.AudioServer.Components.AudioPlayer
{
    class PlaylistAudioPlayer : IPlaylistAudioPlayer, IDisposable
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
            int nextSoundIndex = _currentSoundIndex + 1;
            if (nextSoundIndex >= _soundList.Length)
            {
                nextSoundIndex = 0;
            }
            loadSound(nextSoundIndex);
        }

        private void loadSound(int soundIndex)
        {
            _currentSoundIndex = soundIndex;
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

        public void EnableSoundCard(int index)
        {
            if (_currentSound == null)
            {
                _currentSound = new Sound(_soundProvider.GetSoundData(_currentSoundIndex));
                _currentSound.Completed += currentSoundOnCompleted;
            }
            _currentSound.EnableSoundCard(index);
        }

        public void DisableSoundCard(int index)
        {
            if (_currentSound == null)
            {
                return;
            }
            _currentSound.DisableSoundCard(index);
        }

        public void Play(int index)
        {
            loadSound(index);
            _currentSound.Play();
        }

        public void Dispose()
        {
            if (_currentSound != null)
            {
                _currentSound.Completed -= currentSoundOnCompleted;
                _currentSound.Dispose();
            }
        }
    }
}

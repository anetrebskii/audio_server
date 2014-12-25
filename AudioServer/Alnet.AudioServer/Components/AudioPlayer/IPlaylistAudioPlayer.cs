namespace Alnet.AudioServer.Components.AudioPlayer
{
    interface IPlaylistAudioPlayer : IAudioPlayer
    {
        void Play(int index);
       int GetCurrentSoundIndex();
       SoundInfo[] GetPlayList();
    }
}

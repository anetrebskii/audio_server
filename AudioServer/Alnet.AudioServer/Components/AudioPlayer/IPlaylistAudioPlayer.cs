using System.Security.Cryptography.X509Certificates;

namespace Alnet.AudioServer.Components.AudioPlayer
{
    interface IPlaylistAudioPlayer : IAudioPlayer
    {
        void Play(int index);
       int GetCurrentSoundIndex();
       SoundInfo[] GetPlayList();

       long CurrentPosition { get; set; }
       long Length { get; }
    }
}

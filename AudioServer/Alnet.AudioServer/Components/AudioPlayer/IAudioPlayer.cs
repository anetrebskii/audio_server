namespace Alnet.AudioServer.Components.AudioPlayer
{
    interface IAudioPlayer
    {
        void Play();
        void Stop();

        void EnableSoundCard(int index);
        void DisableSoundCard(int index);
    }
}

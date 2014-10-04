namespace Alnet.AudioServer.Components.AudioPlayer
{
    internal interface ISoundProvider
    {
        SoundInfo[] GetSoundList();
        byte[] GetSoundData(int index);
    }
}
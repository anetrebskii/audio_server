namespace Alnet.AudioServer.AudioPlayer
{
    internal interface ISoundProvider
    {
        SoundInfo[] GetSoundList();
        byte[] GetSoundData(int index);
    }
}
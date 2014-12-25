using System;

namespace Alnet.AudioServer.Components.AudioPlayer
{
    internal interface ISoundProvider
    {
        SoundInfo[] GetSoundList();
        event EventHandler SoundListChanged;
    }
}
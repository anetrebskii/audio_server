using System;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal interface IPlaylistSoundProvider
    {
        SoundInfo[] GetSounds();

        event EventHandler SoundsChanged;
    }
}
using System;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal interface IAudioPlayerController
    {
        AudioPlayerInfo CreatePlaylistAudioPlayer(string name, IPlaylistSoundProvider playlistSoundProvider);

        AudioPlayerInfo[] GetAllAudioPlayers();

        void DeleteAudioPlayer(Guid id);

        AudioPlayerInfo GetAudioPlayer(Guid id);

        ChannelInfo[] GetChannels();
    }
}
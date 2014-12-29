using System;
using System.Collections.Generic;
using System.Linq;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.Common;
using NAudio.Wave;

namespace Alnet.AudioServer.Components.NAudioServer
{
    internal sealed class AudioPlayerController : IDisposable, IAudioPlayerController
    {
        #region Private fields

        /// <summary>
        /// Available audio players
        /// </summary>
        private readonly List<AudioPlayerInfo> _audioPlayers = new List<AudioPlayerInfo>();

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(AudioPlayerController));

        #endregion

        #region IAudioPlayerController

        public AudioPlayerInfo CreatePlaylistAudioPlayer(string name, IPlaylistSoundProvider playlistSoundProvider)
        {
            _disposedGuard.Check();
            Guard.EnsureArgumentNotNullOrWhiteSpace(name, "name");
            Guard.EnsureArgumentNotNull(playlistSoundProvider, "playlistSoundProvider");
            AudioPlayerInfo audioPlayerInfo = new AudioPlayerInfo()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Player = new PlaylistAudioPlayer(playlistSoundProvider)
            };
            _audioPlayers.Add(audioPlayerInfo);
            return audioPlayerInfo;
        }

        public AudioPlayerInfo[] GetAllAudioPlayers()
        {
            _disposedGuard.Check();
            return _audioPlayers.ToArray();
        }

        public void DeleteAudioPlayer(Guid id)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayerToRemove = _audioPlayers.Single(a => a.Id == id);
            _audioPlayers.Remove(audioPlayerToRemove);
            audioPlayerToRemove.Player.DisposeObject();
        }

        public AudioPlayerInfo GetAudioPlayer(Guid id)
        {
            _disposedGuard.Check();
            return _audioPlayers.Single(a => a.Id == id);
        }

        public ChannelInfo[] GetChannels()
        {
            _disposedGuard.Check();
            List<ChannelInfo> returnValue = new List<ChannelInfo>();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                returnValue.Add(new ChannelInfo(i, WaveOut.GetCapabilities(i).ProductName));
            }
            return returnValue.ToArray();
        } 

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _disposedGuard.Dispose();
            _audioPlayers.Select(p => p.Player).DisposeCollection();
            _audioPlayers.Clear();
        } 

        #endregion
    }
}

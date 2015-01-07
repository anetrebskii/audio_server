using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.AudioServer.Components.NAudioServer.Exceptions;
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
        /// The synchronize objecto for <see cref="_audioPlayers"/>.
        /// </summary>
        private readonly object _syncAudioPlayers = new object();

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(AudioPlayerController));

        private readonly AudioServerConfiguration _audioServerConfiguration;

        #endregion

        public AudioPlayerController()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AudioServerConfiguration audioServerConfiguration = configuration.GetSection("audioServer") as AudioServerConfiguration;
            _audioServerConfiguration = audioServerConfiguration ?? new AudioServerConfiguration();
        }

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
            lock (_syncAudioPlayers)
            {
                _audioPlayers.Add(audioPlayerInfo);
            }
            return audioPlayerInfo;
        }

        public AudioPlayerInfo[] GetAllAudioPlayers()
        {
            _disposedGuard.Check();
            lock (_syncAudioPlayers)
            {
                return _audioPlayers.ToArray();
            }
        }

        public void DeleteAudioPlayer(Guid id)
        {
            _disposedGuard.Check();
            lock (_syncAudioPlayers)
            {
                AudioPlayerInfo audioPlayerToRemove = _audioPlayers.FirstOrDefault(a => a.Id == id);
                if (audioPlayerToRemove != null)
                {
                    _audioPlayers.Remove(audioPlayerToRemove);
                    audioPlayerToRemove.Player.DisposeObject();
                }
            }            
        }

        public AudioPlayerInfo GetAudioPlayer(Guid id)
        {
            _disposedGuard.Check();
            lock (_syncAudioPlayers)
            {
                return _audioPlayers.SingleOrDefault(a => a.Id == id);
            }
        }

        public ChannelInfo[] GetChannels()
        {
            _disposedGuard.Check();
            ChannelInfo[] result = new ChannelInfo[0];
            bool isSuccessGetChannels = false;
            do
            {
                try
                {
                    result = getChannelsInternal();
                    isSuccessGetChannels = true;
                }
                catch (ChannelsChangedException)
                {
                    // nothing
                }
            }
            while (!isSuccessGetChannels);
            return result;
        }        

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _disposedGuard.Dispose();
            lock (_syncAudioPlayers)
            {
                _audioPlayers.Select(p => p.Player).DisposeCollection();
                _audioPlayers.Clear();
            }
        }

        #endregion

        #region Private methods

        private ChannelInfo[] getChannelsInternal()
        {
            List<ChannelInfo> returnValue = new List<ChannelInfo>();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var channelName = getChannelName(i);
                var nativeChannelName = getChannelNativeName(i);
                returnValue.Add(new ChannelInfo(i, nativeChannelName, channelName));
            }
            return returnValue.ToArray();
        }

        private static string getChannelNativeName(int channelIndex)
        {
            string nativeChannelName;
            try
            {
                nativeChannelName = WaveOut.GetCapabilities(channelIndex).ProductName;
            }
            catch (Exception ex)
            {
                Guard.RethrowIfFatal(ex);
                if (channelIndex < WaveOut.DeviceCount)
                {
                    nativeChannelName = "<error getting name>";
                }
                else
                {
                    throw new ChannelsChangedException();
                }
            }
            return nativeChannelName;
        }

        private string getChannelName(int channelIndex)
        {
            string channelName = String.Format("Channel-{0}", channelIndex);
            foreach (ChannelElement channel in _audioServerConfiguration.Channels)
            {
                if (channel.Index == channelIndex)
                {
                    channelName = channel.Name;
                    break;
                }
            }
            return channelName;
        }

        #endregion
    }
}

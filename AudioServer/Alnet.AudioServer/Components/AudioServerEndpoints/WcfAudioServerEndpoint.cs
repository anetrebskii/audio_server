using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.AudioServer.Components.NAudioServer;
using Alnet.AudioServerContract;
using Alnet.AudioServerContract.DTO;
using Alnet.Common;

namespace Alnet.AudioServer.Components.AudioServerEndpoints
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class WcfAudioServerEndpoint : IAudioServerService, IAudioServerEndpoint
    {
        #region Private fields

        private readonly IAudioPlayerController _audioPlayerController = ApplicationContext.Instance.AudioPlayerController;
        private readonly ServiceHost _serviceHost;

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(WcfAudioServerEndpoint));

        #endregion

        #region Constructors

        public WcfAudioServerEndpoint()
        {
            _serviceHost = new ServiceHost(this);
        }

        #endregion

        #region IAudioServerEndpoint members

        public void Start()
        {
            _disposedGuard.Check();
            _serviceHost.Open();
        }

        #endregion

        #region IAudioServerService members

        void IAudioServerService.RemoveAudioPlayer(Guid playerId)
        {
            _disposedGuard.Check();
            _audioPlayerController.DeleteAudioPlayer(playerId);
        }

        ChannelDTO[] IAudioServerService.GetAllChannels()
        {
            _disposedGuard.Check();
            return _audioPlayerController
                .GetChannels()
                .Select(convertToDTO)
                .ToArray();
        }

        ChannelDTO[] IAudioServerService.GetEnabledChannels(Guid playerId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayerInfo = _audioPlayerController.GetAudioPlayer(playerId);
            ChannelInfo[] channels = _audioPlayerController.GetChannels();
            return audioPlayerInfo.Player
                .GetEnabledChannels()
                .Select(channelIndex => convertToDTO(channels[channelIndex]))
                .ToArray();
        }

        PlaybackPositionDTO IAudioServerService.GetPlaybackPosition(Guid playerId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayerInfo = _audioPlayerController.GetAudioPlayer(playerId);
            IPlaylistAudioPlayer playlistAudioPlayer = audioPlayerInfo.Player as IPlaylistAudioPlayer;
            if (playlistAudioPlayer != null)
            {
                SoundInfo[] sounds = playlistAudioPlayer.GetSounds();
                int soundIndex = playlistAudioPlayer.GetCurrentSoundIndex();
                return new PlaybackPositionDTO()
                       {              
                           SoundName = soundIndex > -1 && soundIndex < sounds.Length ? sounds[soundIndex].Name : null,
                           SoundIndex = soundIndex,
                           PlaybackPosition = playlistAudioPlayer.CurrentSoundDuration > 0 ?
                                    playlistAudioPlayer.PlaybackPosition / playlistAudioPlayer.CurrentSoundDuration
                                    : 0
                       };
            }
            throw new InvalidOperationException("Player not support playlist");
        }

        SoundDTO[] IAudioServerService.GetSounds(Guid playerId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayerInfo = _audioPlayerController.GetAudioPlayer(playerId);
            IPlaylistAudioPlayer playlistAudioPlayer = audioPlayerInfo.Player as IPlaylistAudioPlayer;
            if (playlistAudioPlayer != null)
            {
                return playlistAudioPlayer
                    .GetSounds()
                    .Select(convertToDTO)
                    .ToArray();
            }
            throw new InvalidOperationException("Player not support playlist");
        }

        AudioPlayerDTO[] IAudioServerService.GetAudioPlayes()
        {
            _disposedGuard.Check();
            return _audioPlayerController
                .GetAllAudioPlayers()
                .Select(convertToDTO)
                .ToArray();
        }

        AudioPlayerDTO IAudioServerService.CreateFileAudioPlayer(string name, string directoryPath)
        {
            _disposedGuard.Check();
            AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, new DirectoryPlaylistSoundProvider(directoryPath));
            return convertToDTO(newAudioPlayerInfo);
        }

        AudioPlayerDTO IAudioServerService.CreateVKAudioPlayer(string name, int vkProfileId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, new VkPlaylistSoundProvider(vkProfileId));
            return convertToDTO(newAudioPlayerInfo);
        }

        AudioPlayerDTO IAudioServerService.GetAudioPlayer(Guid playerId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayer = _audioPlayerController.GetAudioPlayer(playerId);
            return convertToDTO(audioPlayer);
        }

        void IAudioServerService.Play(Guid playerId)
        {
            _disposedGuard.Check();
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Play();
        }

        void IAudioServerService.PlayConcrete(Guid playerId, int soundIndex)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayerInfo = _audioPlayerController.GetAudioPlayer(playerId);
            IPlaylistAudioPlayer playlistAudioPlayer = audioPlayerInfo.Player as IPlaylistAudioPlayer;
            if (playlistAudioPlayer != null)
            {
                playlistAudioPlayer.Play(soundIndex);
            }
            throw new InvalidOperationException("Player not support playlist");
        }

        void IAudioServerService.Stop(Guid playerId)
        {
            _disposedGuard.Check();
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Stop();
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            _disposedGuard.Dispose();
            _serviceHost.Close();
        }

        #endregion

        #region Convert to DTO methods

        private AudioPlayerDTO convertToDTO(AudioPlayerInfo audioPlayerInfo)
        {
            return new AudioPlayerDTO()
            {
                Id = audioPlayerInfo.Id,
                Name = audioPlayerInfo.Name,
                Type = audioPlayerInfo.Player is IPlaylistAudioPlayer ? PlayerTypes.Playlist : PlayerTypes.Stream
            };
        }

        private SoundDTO convertToDTO(SoundInfo soundInfo)
        {
            return new SoundDTO()
            {
                Name = soundInfo.Name
            };
        }

        private ChannelDTO convertToDTO(ChannelInfo channelInfo)
        {
            return new ChannelDTO()
            {
                Index = channelInfo.Index,
                Name = channelInfo.Desciption
            };
        }

        #endregion
    }
}

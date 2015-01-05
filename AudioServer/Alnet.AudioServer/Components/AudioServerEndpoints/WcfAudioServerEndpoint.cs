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
    class WcfAudioServerEndpoint : IAudioPlayerService, IAudioServerEndpoint
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

        #region IAudioPlayerService members

        public PlaylistAudioPlayerDTO[] GetAudioPlayes()
        {
            _disposedGuard.Check();
            return _audioPlayerController
                .GetAllAudioPlayers()
                .Select(convertToDTO)
                .ToArray();
        }

        public PlaylistAudioPlayerDTO CreateFileAudioPlayer(string name, string directoryPath)
        {
            _disposedGuard.Check();
            AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, new DirectoryPlaylistSoundProvider(directoryPath));
            return convertToDTO(newAudioPlayerInfo);
        }

        public PlaylistAudioPlayerDTO CreateVKAudioPlayer(string name, int vkProfileId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, new VkPlaylistSoundProvider(vkProfileId));
            return convertToDTO(newAudioPlayerInfo);
        }

        public PlaylistAudioPlayerDTO GetPlaylistAudioPlayer(Guid playerId)
        {
            _disposedGuard.Check();
            AudioPlayerInfo audioPlayer = _audioPlayerController.GetAudioPlayer(playerId);
            return convertToDTO(audioPlayer);
        }

        public void Play(Guid playerId)
        {
            _disposedGuard.Check();
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Play();
        }

        public void PlayConcrete(Guid playerId, int soundId)
        {
            _disposedGuard.Check();
            IPlaylistAudioPlayer audioPlayer = (IPlaylistAudioPlayer)_audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Play(soundId);
        }

        public void Stop(Guid playerId)
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

        private PlaylistAudioPlayerDTO convertToDTO(AudioPlayerInfo audioPlayerInfo)
        {
            ChannelInfo[] channels = _audioPlayerController.GetChannels();
            IPlaylistAudioPlayer playlistAudioPlayer = (IPlaylistAudioPlayer)audioPlayerInfo.Player;
            return new PlaylistAudioPlayerDTO()
            {
                Id = audioPlayerInfo.Id,
                Name = audioPlayerInfo.Name,
                CurrentSoundIndex = playlistAudioPlayer.GetCurrentSoundIndex(),
                Sounds = playlistAudioPlayer.GetSounds().Select(convertToDTO).ToArray(),
                Channels = playlistAudioPlayer.GetEnabledChannels().Select(channelIndex => convertToDTO(channels[channelIndex])).ToArray()
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

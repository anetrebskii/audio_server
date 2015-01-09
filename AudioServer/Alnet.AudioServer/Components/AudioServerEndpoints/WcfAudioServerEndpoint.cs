using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly Dictionary<PlayerExceptionTypes, FaultCodes> _mappingErrorCodes = new Dictionary<PlayerExceptionTypes, FaultCodes>(); 

        #endregion

        #region Constructors

        public WcfAudioServerEndpoint()
        {
            _serviceHost = new ServiceHost(this);

            _mappingErrorCodes.Add(PlayerExceptionTypes.NoSounds, FaultCodes.NoSounds);
            _mappingErrorCodes.Add(PlayerExceptionTypes.NoSoundCards, FaultCodes.NoChannels);
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
            handleRequest(() => _audioPlayerController.DeleteAudioPlayer(playerId));
        }

        ChannelDTO[] IAudioServerService.GetAllChannels()
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                          {
                              return _audioPlayerController
                                  .GetChannels()
                                  .Select(convertToDTO)
                                  .ToArray();
                          });
        }

        ChannelDTO[] IAudioServerService.GetEnabledChannels(Guid playerId)
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                          {
                              AudioPlayerInfo audioPlayerInfo = getAudioPlayerInfo(playerId);
                              ChannelInfo[] allChannels = _audioPlayerController.GetChannels();
                              return audioPlayerInfo.Player
                                  .GetEnabledChannels()
                                  .Where(channelIndex => channelIndex < allChannels.Length)
                                  .Select(channelIndex => convertToDTO(allChannels[channelIndex]))
                                  .ToArray();
                          });
        }

        PlaybackPositionDTO IAudioServerService.GetPlaybackPosition(Guid playerId)
        {
            //TODO: Provide correct playback position
            _disposedGuard.Check();
            return handleRequest(() =>
                                 {
                                     IPlaylistAudioPlayer playlistAudioPlayer = getPlaylistAudioPlayer(playerId);
                                     SoundInfo[] sounds = playlistAudioPlayer.GetSounds();
                                     if (sounds.Length == 0)
                                     {
                                         throw new FaultException<FaultCodes>(FaultCodes.NoSounds);
                                     }
                                     int soundIndex = playlistAudioPlayer.CurrentSoundIndex;
                                     SoundInfo soundInfo = playlistAudioPlayer.CurrentSound;
                                     string soundName = soundInfo == null ? null : soundInfo.Name;
                                     return new PlaybackPositionDTO()
                                            {
                                                IsPlaying = playlistAudioPlayer.IsPlaying,
                                                SoundName = soundName,
                                                SoundIndex = soundIndex,
                                                PlaybackPosition = playlistAudioPlayer.PlaybackPosition
                                            };
                                 });
        }

        SoundDTO[] IAudioServerService.GetSounds(Guid playerId)
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                                 {
                                     IPlaylistAudioPlayer playlistAudioPlayer = getPlaylistAudioPlayer(playerId);
                                     return playlistAudioPlayer
                                         .GetSounds()
                                         .Select(convertToDTO)
                                         .ToArray();
                                 });
        }

        AudioPlayerDTO[] IAudioServerService.GetAudioPlayes()
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                                 {
                                     return _audioPlayerController
                                         .GetAllAudioPlayers()
                                         .Select(convertToDTO)
                                         .ToArray();
                                 });
        }

        AudioPlayerDTO IAudioServerService.CreateFileAudioPlayer(string name, string directoryPath)
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                          {
                              IPlaylistSoundProvider soundProvider = new DirectoryPlaylistSoundProvider(directoryPath);
                              AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, soundProvider);
                              return convertToDTO(newAudioPlayerInfo);
                          });
        }

        AudioPlayerDTO IAudioServerService.CreateVKAudioPlayer(string name, int vkProfileId)
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                                 {
                                     IPlaylistSoundProvider soundProvider = new VkPlaylistSoundProvider(vkProfileId);
                                     AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, soundProvider);
                                     return convertToDTO(newAudioPlayerInfo);
                                 });
        }

        AudioPlayerDTO IAudioServerService.GetAudioPlayer(Guid playerId)
        {
            _disposedGuard.Check();
            return handleRequest(() =>
                                 {
                                     AudioPlayerInfo audioPlayer = getAudioPlayerInfo(playerId);
                                     return convertToDTO(audioPlayer);
                                 });
        }

        void IAudioServerService.Play(Guid playerId)
        {
            _disposedGuard.Check();
            handleRequest(() =>
                          {
                              IAudioPlayer audioPlayer = getAudioPlayerInfo(playerId).Player;
                              audioPlayer.Play();
                          });
        }

        void IAudioServerService.PlayConcrete(Guid playerId, int soundIndex)
        {
            _disposedGuard.Check();
            handleRequest(() =>
                          {
                              IPlaylistAudioPlayer playlistAudioPlayer = getPlaylistAudioPlayer(playerId);
                              playlistAudioPlayer.Play(soundIndex);
                          });
        }

        public void MoveNextSound(Guid playerId)
        {
            _disposedGuard.Check();
            handleRequest(() =>
                          {
                              IPlaylistAudioPlayer playlistAudioPlayer = getPlaylistAudioPlayer(playerId);
                              playlistAudioPlayer.Play(playlistAudioPlayer.CurrentSoundIndex + 1);
                          });
        }

        public void MovePrevSound(Guid playerId)
        {
            _disposedGuard.Check();
            handleRequest(() =>
                          {
                              IPlaylistAudioPlayer playlistAudioPlayer = getPlaylistAudioPlayer(playerId);
                              playlistAudioPlayer.Play(playlistAudioPlayer.CurrentSoundIndex - 1);
                          });
        }

        void IAudioServerService.Stop(Guid playerId)
        {
            _disposedGuard.Check();
            handleRequest(() =>
                          {
                              IAudioPlayer audioPlayer = getAudioPlayerInfo(playerId).Player;
                              audioPlayer.Stop();
                          });
        }

        void IAudioServerService.ChangeChannelState(Guid playerId, int channelIndex, bool newState)
        {
            handleRequest(() =>
                          {
                              IAudioPlayer audioPlayer = getAudioPlayerInfo(playerId).Player;
                              if (newState)
                              {
                                  audioPlayer.EnableChannel(channelIndex);
                              }
                              else
                              {
                                  audioPlayer.DisableChannel(channelIndex);
                              }
                          });
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

        private TReturn handleRequest<TReturn>(Func<TReturn> callback)
        {
            try
            {
                return callback();
            }
            catch (Exception ex)
            {
                Guard.RethrowIfFatal(ex);
                handleResponseError(ex);
                return default(TReturn);
            }
        }

        private void handleRequest(Action callback)
        {
            try
            {
                callback();
            }                
            catch (Exception ex)
            {
                Guard.RethrowIfFatal(ex);
                handleResponseError(ex);
            }
        }

        private void handleResponseError(Exception ex)
        {
            if (ex is FaultException<FaultCodes>)
            {
                throw ex;
            }
            if (ex is PlayerException)
            {
                PlayerException playerException = (PlayerException)ex;
                FaultCodes faultCode;
                if (_mappingErrorCodes.TryGetValue(playerException.Type, out faultCode))
                {
                    throw new FaultException<FaultCodes>(faultCode);
                }
            }
            throw new FaultException<FaultCodes>(FaultCodes.Uknown);
        }

        private AudioPlayerInfo getAudioPlayerInfo(Guid playerId)
        {
            AudioPlayerInfo audioPlayerInfo = _audioPlayerController.GetAudioPlayer(playerId);
            if (audioPlayerInfo == null)
            {
                throw new FaultException<FaultCodes>(FaultCodes.NoPlayer);
            }
            return audioPlayerInfo;
        }

        private IPlaylistAudioPlayer getPlaylistAudioPlayer(Guid playerId)
        {
            AudioPlayerInfo audioPlayerInfo = getAudioPlayerInfo(playerId);
            IPlaylistAudioPlayer playlistAudioPlayer = audioPlayerInfo.Player as IPlaylistAudioPlayer;
            if (playlistAudioPlayer == null)
            {
                throw new FaultException<FaultCodes>(FaultCodes.IncorrectPlayerType);
            }
            return playlistAudioPlayer;
        }

        #endregion
    }
}

using System;
using System.ServiceModel;

using Alnet.AudioServer.Web.AudioServerService;
using Alnet.AudioServerContract;
using Alnet.AudioServerContract.DTO;
using Alnet.Common;

using IAudioServerService = Alnet.AudioServer.Web.AudioServerService.IAudioServerService;

namespace Alnet.AudioServer.Web.Components.AudioServerService
{
    public class AudioServerServiceProxy : IAudioServerService, IDisposable
    {
        #region Private fields

        /// <summary>
        /// The audio server service client
        /// </summary>
        private AudioServerServiceClient _service;

        /// <summary>
        /// The sync service
        /// </summary>
        private readonly object _syncService = new object();

        /// <summary>
        /// The disposed guard
        /// </summary>
        private readonly DisposedGuard _disposedGuard = new DisposedGuard(typeof(AudioServerServiceProxy)); 

        #endregion

        #region IAudioServerService

        public AudioPlayerDTO CreateFileAudioPlayer(string name, string directoryPath)
        {
            return invokeService((service) => service.CreateFileAudioPlayer(name, directoryPath));
        }

        public AudioPlayerDTO CreateVKAudioPlayer(string name, int vkProfileId)
        {
            return invokeService((service) => service.CreateVKAudioPlayer(name, vkProfileId));
        }

        public void RemoveAudioPlayer(Guid playerId)
        {
            invokeService((service) => service.RemoveAudioPlayer(playerId));
        }

        public AudioPlayerDTO[] GetAudioPlayes()
        {
            return invokeService((service) => service.GetAudioPlayes());
        }

        public ChannelDTO[] GetAllChannels()
        {
            return invokeService((service) => service.GetAllChannels());
        }

        public ChannelDTO[] GetEnabledChannels(Guid playerId)
        {
            return invokeService((service) => service.GetEnabledChannels(playerId));
        }

        public PlaybackPositionDTO GetPlaybackPosition(Guid playerId)
        {
            return invokeService((service) => service.GetPlaybackPosition(playerId));
        }

        public SoundDTO[] GetSounds(Guid playerId)
        {
            return invokeService((service) => service.GetSounds(playerId));
        }

        public AudioPlayerDTO GetAudioPlayer(Guid playerId)
        {
            return invokeService((service) => service.GetAudioPlayer(playerId));
        }

        public void Play(Guid playerId)
        {
            invokeService((service) => service.Play(playerId));
        }

        public void PlayConcrete(Guid playerId, int soundId)
        {
            invokeService((service) => service.PlayConcrete(playerId, soundId));
        }

        public void MoveNextSound(Guid playerId)
        {
            invokeService((service) => service.MoveNextSound(playerId));
        }

        public void MovePrevSound(Guid playerId)
        {
            invokeService((service) => service.MovePrevSound(playerId));
        }

        public void Stop(Guid playerId)
        {
            invokeService((service) => service.Stop(playerId));
        }

        public void ChangeChannelState(Guid playerId, int channelIndex, bool newState)
        {
            invokeService((service) => service.ChangeChannelState(playerId, channelIndex, newState));
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            _disposedGuard.Dispose();
            if (_service != null)
            {
                if (_service.State == CommunicationState.Opened)
                {
                    _service.Close();
                }
                else
                {
                    _service.Abort();
                }
                _service = null;
            }
        }

        #endregion

        #region Private fields

        private void invokeService(Action<IAudioServerService> callback)
        {
            _disposedGuard.Check();
            provideChannel();
            try
            {
                callback(_service);
            }
            catch (Exception ex)
            {
                Guard.RethrowIfFatal(ex);
                if (ex is FaultException<FaultCodes>)
                {
                    throw;
                }
                throw new ServiceException("Uknown exception", ex);
            }
        }

        private TReturnValue invokeService<TReturnValue>(Func<IAudioServerService, TReturnValue> callback)
        {
            _disposedGuard.Check();
            provideChannel();
            try
            {
                return callback(_service);
            }
            catch (Exception ex)
            {
                Guard.RethrowIfFatal(ex);
                if (ex is FaultException<FaultCodes>)
                {
                    throw;
                }
                throw new ServiceException("Uknown exception", ex);
            }
        }

        private void provideChannel()
        {
            lock (_syncService)
            {
                if (_service == null
                    || _service.State == CommunicationState.Closed)
                {
                    _service = new AudioServerServiceClient();
                }
                else if (_service.State == CommunicationState.Faulted)
                {
                    _service.Abort();
                    _service = new AudioServerServiceClient();
                }

                if (_service.State == CommunicationState.Created)
                {
                    try
                    {
                        _service.Open();
                    }
                    catch (Exception ex)
                    {
                        Guard.RethrowIfFatal(ex);
                        throw new ServiceException("Connection is not opened");
                    }
                }
            }
            if (_service.State != CommunicationState.Opened)
            {
                throw new ServiceException("Connection is not opened");
            }
        } 

        #endregion
    }
}
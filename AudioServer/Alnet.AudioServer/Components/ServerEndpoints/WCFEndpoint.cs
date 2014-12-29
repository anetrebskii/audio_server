using System;
using System.Linq;
using System.ServiceModel;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.AudioServer.Components.NAudioServer;
using Alnet.AudioServer.Components.ServerEndpoints.Impl;
using Alnet.AudioServerContract;
using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServer.Components.ServerEndpoints
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class WCFEndpoint : IAudioPlayerService, IEndpoint
    {
        private readonly AudioPlayerController _audioPlayerController = new AudioPlayerController();
        private readonly ServiceHost _serviceHost;

        public WCFEndpoint()
        {
            _serviceHost = new ServiceHost(this, new Uri("net.pipe://localhost/audioplayer"));      
        }

        public void Start()
        {
            _serviceHost.Open();
        }

        public AudioPlayerInfoDTO[] GetAudioPlayes()
        {
            return _audioPlayerController
                .GetAllAudioPlayers()
                .Select(convertToDTO)
                .ToArray();
        }

        public AudioPlayerInfoDTO CreateFileAudioPlayer(string name, string directoryPath)
        {
            AudioPlayerInfo newAudioPlayerInfo = _audioPlayerController.CreatePlaylistAudioPlayer(name, new DirectoryPlaylistSoundProvider(directoryPath));
            return convertToDTO(newAudioPlayerInfo);
        }

        public void Play(Guid playerId)
        {
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Play();
        }

        public void PlayConcrete(Guid playerId, int soundId)
        {
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            ((IPlaylistAudioPlayer)audioPlayer).Play(soundId);
        }

        public void Stop(Guid playerId)
        {
            IAudioPlayer audioPlayer = _audioPlayerController.GetAudioPlayer(playerId).Player;
            audioPlayer.Stop();
        }

        private AudioPlayerInfoDTO convertToDTO(AudioPlayerInfo audioPlayerInfo)
        {
            return new AudioPlayerInfoDTO()
            {
                Id = audioPlayerInfo.Id,
                Name = audioPlayerInfo.Name
            };
        }

        public void Dispose()
        {
            _serviceHost.Close();
        }
    }
}

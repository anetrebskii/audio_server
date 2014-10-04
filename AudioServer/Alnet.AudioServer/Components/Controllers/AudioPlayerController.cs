using System;
using System.Collections.Generic;
using System.Linq;
using Alnet.AudioServer.Components.AudioPlayer;

namespace Alnet.AudioServer.Components.Controllers
{
    class AudioPlayerController
    {
        private readonly List<AudioPlayerInfo> _audioPlayers = new List<AudioPlayerInfo>(); 

        public AudioPlayerInfo CreateAudioPlayer(string name, ISoundProvider soundProvider)
        {
            AudioPlayerInfo audioPlayerInfo = new AudioPlayerInfo()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Player = new PlaylistAudioPlayer(soundProvider)
            };
            _audioPlayers.Add(audioPlayerInfo);
            return audioPlayerInfo;
        }        

        public AudioPlayerInfo[] GetAllAudioPlayers()
        {
            return _audioPlayers.ToArray();
        }

        public void DeleteAudioPlayer(Guid id)
        {
            AudioPlayerInfo audioPlayerToRemove = _audioPlayers.Single(a => a.Id == id);
            _audioPlayers.Remove(audioPlayerToRemove);
        }

        public AudioPlayerInfo GetAudioPlayer(Guid id)
        {
            return _audioPlayers.Single(a => a.Id == id);
        }
    }
}

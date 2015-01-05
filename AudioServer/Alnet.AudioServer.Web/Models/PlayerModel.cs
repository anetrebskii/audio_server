using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServer.Web.Models
{
    public class PlayerModel
    {
        public PlayerModel()
        {
            PlayList = new List<SoundModel>();
        }

        public SoundModel Current { get; set; }
        public Guid PlayerId { get; set; }

        public List<RoomModel> Rooms { get; set; } 
        public List<SoundModel> PlayList { get; set; }

        public static PlayerModel Parse(PlaylistAudioPlayerDTO audioPlayer)
        {
            return new PlayerModel
            {
                PlayerId = audioPlayer.Id,
                Current = audioPlayer.CurrentSoundIndex > -1
                ? new SoundModel()
                {
                    Name = audioPlayer.Sounds[audioPlayer.CurrentSoundIndex].Name
                }
                : new SoundModel()
                {
                    Name = audioPlayer.Sounds[0].Name
                },
                PlayList = audioPlayer
                    .Sounds
                    .Select(p => new SoundModel()
                    {
                        Name = p.Name
                    })
                    .ToList(),
                Rooms = audioPlayer.Channels
                    .Select(c => new RoomModel()
                    {
                        Id = c.Index,
                        Name = c.Name,
                        IsSelected = true,
                    })
                    .ToList()
            };
        }
    }
}
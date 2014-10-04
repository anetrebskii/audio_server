using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}
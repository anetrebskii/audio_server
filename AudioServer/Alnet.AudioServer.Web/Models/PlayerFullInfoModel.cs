using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alnet.AudioServer.Web.Models
{
    public class PlayerFullInfoModel
    {
        public PlayerFullInfoModel()
        {
            Channels = new List<ChannelModel>();
            Sounds = new List<string>();
        }

        public Guid PlayerId { get; set; }

        public PlaybackModel Playback { get; set; }

        public List<ChannelModel> Channels { get; set; }

        public List<string> Sounds { get; set; }
    }
}
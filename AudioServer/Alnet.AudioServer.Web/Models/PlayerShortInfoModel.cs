using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServer.Web.Models
{
    public class PlayerShortInfoModel
    {
        public Guid PlayerId { get; set; }

        public string CurrentSoundName { get; set; }

        public List<string> EnabledChannels { get; set; } 
    }
}
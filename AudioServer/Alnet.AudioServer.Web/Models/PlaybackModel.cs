using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alnet.AudioServer.Web.Models
{
    public class PlaybackModel
    {
        public string SoundName { get; set; }

        /// <summary>
        /// Gets or sets the playback position.
        /// </summary>
        /// <remarks>
        /// IN bounds [0..1]
        /// </remarks>
        public double PlaybackPosition { get; set; }
    }
}
using System;
using Alnet.AudioServer.Components.AudioPlayer;

namespace Alnet.AudioServer.Components.Controllers
{
    class AudioPlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IAudioPlayer Player { get; set; }
    }
}

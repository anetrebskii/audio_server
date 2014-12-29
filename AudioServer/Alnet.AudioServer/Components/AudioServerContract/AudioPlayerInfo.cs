using System;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal class AudioPlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IAudioPlayer Player { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer
{
    class AudioPlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IAudioPlayer Player { get; set; }
    }
}

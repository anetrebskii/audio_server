using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer
{
    interface IPlaylistAudioPlayer : IAudioPlayer
    {
        void Play(int index);
    }
}

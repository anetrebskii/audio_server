using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer
{
    interface IAudioPlayer
    {
        void Play();
        void Stop();

        void EnableSoundCard(int index);
        void DisableSoundCard(int index);
    }
}

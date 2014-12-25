using System;
using Alnet.AudioServer.Components.AudioPlayer;

namespace Alnet.AudioServer.Components.SoundProviders
{
    class VKSoundProvider : ISoundProvider
    {
        private readonly int _profileId;

        public VKSoundProvider(int profileId)
        {
            _profileId = profileId;
        }

        public SoundInfo[] GetSoundList()
        {
            throw new NotImplementedException();
        }

        public byte[] GetSoundData(int index)
        {
            throw new NotImplementedException();
        }

       public event EventHandler SoundListChanged;
    }
}

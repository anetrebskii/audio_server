using System;
using Alnet.AudioServer.Components.AudioPlayer;
using ApiCore;
using ApiCore.Audio;

namespace Alnet.AudioServer.Components.SoundProviders
{
    class VKSoundProvider : ISoundProvider
    {
        private readonly int _profileId;

        public VKSoundProvider(int profileId)
        {
            _profileId = profileId;
           SessionInfo sessionInfo = new SessionInfo()
           {
              AppId = 4697962,
              Secret = "lcXPV1RsUi3lWcN3GiyD",              
           };
           ApiManager manager = new ApiManager(sessionInfo);
           AudioFactory factory = new AudioFactory(manager);
           string accessToken = "12d0be0ca68b000729cb9d79ab7feac9aeaaed1a0bdeb2fa247860d1b47b93d0bde923d0f20569029dbf2";
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Alnet.AudioServer.Components.AudioPlayer;
using ApiCore;
using ApiCore.Audio;

namespace Alnet.AudioServer.Components.SoundProviders
{
    class VKSoundProvider : ISoundProvider
    {
        private readonly int _profileId;
        private ApiManager _manager;
        private AudioFactory _factory;

        public VKSoundProvider(int profileId)
        {
            _profileId = profileId;
            SessionInfo sessionInfo = new SessionInfo()
            {
                AccessToken = "12d0be0ca68b000729cb9d79ab7feac9aeaaed1a0bdeb2fa247860d1b47b93d0bde923d0f20569029dbf2"
            };
            _manager = new ApiManager(sessionInfo);
            _factory = new AudioFactory(_manager);
        }

        public SoundInfo[] GetSoundList()
        {
            return _factory
                .Get(_profileId, null, null)
                .Select(a => new SoundInfo()
                {
                    Name = a.Title,
                    Url = a.Url
                })
                .ToArray();
        }

        public event EventHandler SoundListChanged;
    }
}

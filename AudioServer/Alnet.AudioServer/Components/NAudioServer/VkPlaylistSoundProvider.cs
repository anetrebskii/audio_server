using System;
using System.Linq;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.Common;
using ApiCore;
using ApiCore.Audio;

namespace Alnet.AudioServer.Components.NAudioServer
{
   /// <summary>
   /// Provide sound playlist from vk.com
   /// </summary>
    internal sealed class VkPlaylistSoundProvider : IPlaylistSoundProvider
    {
       #region Private fields

       /// <summary>
       /// The profile identifier in the vk.com
       /// </summary>
       private readonly int _vkProfileId;

       /// <summary>
       /// Provides access to the audio files in the vk.com
       /// </summary>
       private readonly AudioFactory _vkAudioProvider;

       #endregion

       #region Constructors

       /// <summary>
       /// Initializes a new instance of the <see cref="VkPlaylistSoundProvider"/> class.
       /// </summary>
       /// <param name="vkProfileId">The profile identifier in the vk.com</param>
       public VkPlaylistSoundProvider(int vkProfileId)
       {
          Guard.VerifyArgument(vkProfileId > 0, "vkProfileId must be more than 0");
          _vkProfileId = vkProfileId;
          _vkAudioProvider = createVkAudioFactory();
       }

      #endregion

        #region IPlaylistSoundProvider members

        public SoundInfo[] GetSounds()
        {
           return _vkAudioProvider
               .Get(_vkProfileId, null, null)
               .Select(a => new SoundInfo(a.Title, a.Url))
               .ToArray();
        }

        public event EventHandler SoundsChanged; 

        #endregion

       #region Private methods

        /// <summary>
        /// Creates the instance for load audios from the vk.com
        /// </summary>
        /// <returns></returns>
        private AudioFactory createVkAudioFactory()
        {
           SessionInfo sessionInfo = new SessionInfo()
           {
              AccessToken = AppSettings.Default.VkAccessToken
           };
           ApiManager vkApiManager = new ApiManager(sessionInfo);
           return new AudioFactory(vkApiManager);
        }
       
       #endregion
    }
}

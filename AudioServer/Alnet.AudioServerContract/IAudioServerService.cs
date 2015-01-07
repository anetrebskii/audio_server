using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServerContract
{
    [ServiceContract(Namespace = "audioserver")]
    public interface IAudioServerService
    {
        #region Creates/removes players

        [OperationContract]
        AudioPlayerDTO CreateFileAudioPlayer(string name, string directoryPath);

        [OperationContract]
        AudioPlayerDTO CreateVKAudioPlayer(string name, int vkProfileId);

        [OperationContract]
        void RemoveAudioPlayer(Guid playerId);

        #endregion

        #region Methods for gets information for players

        [OperationContract]
        AudioPlayerDTO[] GetAudioPlayes();

        [OperationContract]
        ChannelDTO[] GetAllChannels();

        [OperationContract]
        ChannelDTO[] GetEnabledChannels(Guid playerId);

        /// <summary>
        /// Gets the playback position.
        /// </summary>
        /// <param name="playerId">The player identifier.</param>
        /// <remarks>Only for Playlistaudioplayers</remarks>
        [OperationContract]
        PlaybackPositionDTO GetPlaybackPosition(Guid playerId);

        [OperationContract]
        SoundDTO[] GetSounds(Guid playerId);

        [OperationContract]
        AudioPlayerDTO GetAudioPlayer(Guid playerId);

        #endregion

        #region Commands for players

        [OperationContract]
        void Play(Guid playerId);

        [OperationContract]
        void PlayConcrete(Guid playerId, int soundId);

        [OperationContract]
        void Stop(Guid playerId); 

        #endregion
    }
}

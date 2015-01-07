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
        [FaultContract(typeof(FaultCodes))]
        AudioPlayerDTO CreateFileAudioPlayer(string name, string directoryPath);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        AudioPlayerDTO CreateVKAudioPlayer(string name, int vkProfileId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void RemoveAudioPlayer(Guid playerId);

        #endregion

        #region Methods for gets information for players

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        AudioPlayerDTO[] GetAudioPlayes();

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        ChannelDTO[] GetAllChannels();

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        ChannelDTO[] GetEnabledChannels(Guid playerId);

        /// <summary>
        /// Gets the playback position.
        /// </summary>
        /// <param name="playerId">The player identifier.</param>
        /// <remarks>Only for Playlistaudioplayers</remarks>
        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        PlaybackPositionDTO GetPlaybackPosition(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        SoundDTO[] GetSounds(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        AudioPlayerDTO GetAudioPlayer(Guid playerId);

        #endregion

        #region Commands for players

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void Play(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void PlayConcrete(Guid playerId, int soundId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void MoveNextSound(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void MovePrevSound(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void Stop(Guid playerId);

        [OperationContract]
        [FaultContract(typeof(FaultCodes))]
        void ChangeChannelState(Guid playerId, int channelIndex, bool newState);

        #endregion
    }
}

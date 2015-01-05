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
    public interface IAudioPlayerService
    {
        [OperationContract]
        PlaylistAudioPlayerDTO[] GetAudioPlayes();

        [OperationContract]
        PlaylistAudioPlayerDTO CreateFileAudioPlayer(string name, string directoryPath);

        [OperationContract]
        PlaylistAudioPlayerDTO CreateVKAudioPlayer(string name, int vkProfileId);

        [OperationContract]
        PlaylistAudioPlayerDTO GetPlaylistAudioPlayer(Guid playerId);

        [OperationContract]
        void Play(Guid playerId);

        [OperationContract]
        void PlayConcrete(Guid playerId, int soundId);

        [OperationContract]
        void Stop(Guid playerId);
    }
}

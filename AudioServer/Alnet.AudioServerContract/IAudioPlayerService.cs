using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServerContract
{
    [ServiceContract()]
    public interface IAudioPlayerService
    {
        [OperationContract]
        AudioPlayerInfoDTO[] GetAudioPlayes();

        [OperationContract]
        AudioPlayerInfoDTO CreateFileAudioPlayer(string name, string directoryPath);

        [OperationContract]
        void Play(Guid playerId);

        [OperationContract]
        void PlayConcrete(Guid playerId, int soundId);

        [OperationContract]
        void Stop(Guid playerId);
    }
}

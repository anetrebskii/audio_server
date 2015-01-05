using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public class PlaylistAudioPlayerDTO : AudioPlayerDTO
    {        
        [DataMember]
        public int CurrentSoundIndex { get; set; }

        [DataMember]
        public SoundDTO[] Sounds { get; set; }
    }
}

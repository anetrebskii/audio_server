using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public class AudioPlayerInfoDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SoundDTO[] Playlist { get; set; }
    }
}

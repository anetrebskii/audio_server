using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public class AudioPlayerDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ChannelDTO[] Channels { get; set; }
    }
}
using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public enum PlayerTypes
    {
        [EnumMember]
        Playlist,

        [EnumMember]
        Stream
    }

    [DataContract]
    public sealed class AudioPlayerDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public PlayerTypes Type { get; set; }
    }
}
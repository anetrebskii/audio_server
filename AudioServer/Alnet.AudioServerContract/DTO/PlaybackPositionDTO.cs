using System.Runtime.Serialization;

using JetBrains.Annotations;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public sealed class PlaybackPositionDTO
    {
        [DataMember]
        public string SoundName { get; set; }

        /// <summary>
        /// Gets or sets the index of the sound.
        /// </summary>
        /// <returns>
        /// <c>-1</c> if not set current sound.
        /// </returns>
        [DataMember]
        public int SoundIndex { get; set; }

        /// <summary>
        /// Gets or sets the duration of the sound.
        /// </summary>
        /// <returns>
        /// In bounds [0..1]
        /// </returns>
        [DataMember]
        public double PlaybackPosition { get; set; }
    }
}

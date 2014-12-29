namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal interface IPlaylistAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// Playback position of the current sound.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        long PlaybackPosition { get; set; }

        /// <summary>
        /// Returns duration of the current playing sound.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        long CurrentSoundDuration { get; }

        /// <summary>
        /// Starts playing the sound with index in the playlist.
        /// </summary>
        /// <param name="soundIndex">Index of the sound in the playlist.</param>
        /// 
        /// <exception cref="PlayerException"/>
        void Play(int soundIndex);

        /// <summary>
        /// Gets the sounds in the playlist.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        SoundInfo[] GetSounds();

        /// <summary>
        /// Returns index of the current sound in the playlist.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        int GetCurrentSoundIndex();
    }
}
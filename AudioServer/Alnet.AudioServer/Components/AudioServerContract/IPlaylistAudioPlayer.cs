namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal interface IPlaylistAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// Playback position of the current sound.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        double PlaybackPosition { get; set; }

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
        int CurrentSoundIndex { get; }

        /// <summary>
        /// Returns information about current sound
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        SoundInfo CurrentSound { get; }
    }
}
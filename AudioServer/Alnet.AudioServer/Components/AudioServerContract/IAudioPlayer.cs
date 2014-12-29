namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal interface IAudioPlayer
    {
        /// <summary>
        /// Starts the playing a sounds.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        void Play();

        /// <summary>
        /// Stops the playing a sounds.
        /// </summary>
        /// 
        /// <exception cref="PlayerException"/>
        void Stop();

        /// <summary>
        /// Enables the audio channel.
        /// </summary>
        /// <param name="index">The index the channel.</param>
        /// 
        /// <exception cref="PlayerException"/>
        void EnableChannel(int index);

        /// <summary>
        /// Disables the audio channel.
        /// </summary>
        /// <param name="index">The index the channel.</param>
        /// 
        /// <exception cref="PlayerException"/>
        void DisableChannel(int index);
    }
}
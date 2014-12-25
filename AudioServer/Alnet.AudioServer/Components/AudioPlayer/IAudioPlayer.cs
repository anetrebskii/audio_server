namespace Alnet.AudioServer.Components.AudioPlayer
{
   internal interface IAudioPlayer
   {
      void Play();
      void Stop();

      ChannelInfo[] GetChannels();
      void EnableChannel(int index);
      void DisableChannel(int index);
   }
}

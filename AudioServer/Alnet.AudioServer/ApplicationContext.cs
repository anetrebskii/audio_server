using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.AudioServer.Components.NAudioServer;

namespace Alnet.AudioServer
{
   internal sealed class ApplicationContext
   {
      #region Private fields

      /// <summary>
      /// The audio player controller
      /// </summary>
      private readonly IAudioPlayerController _audioPlayerController = new AudioPlayerController();

      #endregion

      #region Singleton

      /// <summary>
      /// The singleton instance.
      /// </summary>
      private static readonly ApplicationContext _instance = new ApplicationContext();

      private ApplicationContext()
      {         
      }

      /// <summary>
      /// The singleton instance.
      /// </summary>
      public static ApplicationContext Instance
      {
         get { return _instance; }
      }

      #endregion

      #region Properties

      public IAudioPlayerController AudioPlayerController
      {
         get { return _audioPlayerController; }
      } 

      #endregion
   }
}

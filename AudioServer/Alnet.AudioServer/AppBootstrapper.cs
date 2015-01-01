using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.AudioServer.Components.AudioServerEndpoints;
using Alnet.AudioServer.Components.NAudioServer;
using Alnet.Common;

namespace Alnet.AudioServer
{

   [RunInstaller(true)]
   public sealed class AppBootstrapper : Bootstrapper
   {
      private IAudioServerEndpoint _audioServerEndpoint = new WcfAudioServerEndpoint();

      protected override void OnStart()
      {         
         _audioServerEndpoint.Start();
         debugCode();
      }

      protected override void OnStop()
      {
         _audioServerEndpoint.Dispose();
         _audioServerEndpoint = null;
      }

      protected override string Name
      {
         get { return "Alnet.AudioServer"; }
      }

      protected override string DisplayName
      {
         get { return "Audio Server"; }
      }

      protected override string Description
      {
         get { return "Audio server for playing sounds synchronously on different audio cards."; }
      }

      private void debugCode()
      {
         IAudioPlayerController controller = new AudioPlayerController();

         AudioPlayerInfo playerInfo = controller.CreatePlaylistAudioPlayer("My", new DirectoryPlaylistSoundProvider(@"D:\Music\VKMusic"));
         //AudioPlayerInfo playerInfo = controller.CreatePlaylistAudioPlayer("My", new VkPlaylistSoundProvider(709939));
         

         Console.WriteLine("Available channels");
         foreach (ChannelInfo channelInfo in controller.GetChannels())
         {
            Console.WriteLine(channelInfo);
         }

         IPlaylistAudioPlayer playlistAudioPlayer = (IPlaylistAudioPlayer)playerInfo.Player;

         //Console.WriteLine("Playlist");
         //foreach (var sounds in playlistAudioPlayer.GetSounds())
         //{
         //   Console.WriteLine(sounds.Name);
         //}          
         playlistAudioPlayer.EnableChannel(0);
         playlistAudioPlayer.Play();
         Console.ReadLine();

         playlistAudioPlayer.PlaybackPosition = playlistAudioPlayer.CurrentSoundDuration - 2000000;

         Console.ReadLine();

         playlistAudioPlayer.EnableChannel(1);
         playlistAudioPlayer.Play(playlistAudioPlayer.GetCurrentSoundIndex() + 1);
         //controller.DeleteAudioPlayer(playerInfo.Id);
         Console.ReadLine();

         controller.DisposeObject();
         return;

         Console.ReadLine();
         playerInfo.Player.DisableChannel(0);
         Console.ReadLine();
         return;

         IAudioServerEndpoint audioServerEndpoint = new WcfAudioServerEndpoint();
         audioServerEndpoint.Start();
         Console.ReadLine();
         return;
      }
   }
}

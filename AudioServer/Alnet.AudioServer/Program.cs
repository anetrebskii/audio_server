using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alnet.AudioServer.Components.AudioPlayer;
using Alnet.AudioServer.Components.Controllers;
using Alnet.AudioServer.Components.ServerEndpoints;
using Alnet.AudioServer.Components.ServerEndpoints.Impl;
using Alnet.AudioServer.Components.SoundProviders;
using NAudio.Wave;

namespace Alnet.AudioServer
{
   class AudioProvider : IWaveProvider
   {
      private MemoryStream memoryStream;
      private Mp3FileReader fileReader;
      private Mp3FileReader fileReaderNext;

      public AudioProvider()
      {
         WebClient client = new WebClient();
         byte[] data = client.DownloadData("http://dl.zaycev.net/ptchk/154044/2995562/vremya_i_steklo_-_karoche_a_tyts_tyts_tyts_ty_tseluesh_menya_(zaycev.net).mp3?dlKind=dl");
         memoryStream = new MemoryStream(data);
         fileReader = new Mp3FileReader(memoryStream);

         data = client.DownloadData("http://dl.zaycev.net/681599/2919306/jason_derulo_feat_(zaycev.net)._snoop_dogg_-_wiggle.mp3?dlKind=dl");
         memoryStream = new MemoryStream(data);
         fileReaderNext = new Mp3FileReader(memoryStream);

      }

      public long CurrentPosition
      {
         get { return fileReader.Position; }
      }

      public WaveFormat WaveFormat
      {
         get { return fileReader.WaveFormat; }
      }

      public void SeekTo(long position)
      {
         fileReader.Seek(position, SeekOrigin.Begin);
      }

      public long Length
      {
         get { return fileReader.Length; }
      }

      public int Read(byte[] buffer, int offset, int count)
      {
         int readCount = fileReader.Read(buffer, offset, count);
         if (readCount < count)
         {
            fileReader = fileReaderNext;
         }
         return readCount;
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         AudioPlayerController controller = new AudioPlayerController();
         AudioPlayerInfo playerInfo = controller.CreateAudioPlayer("My", new DirectorySoundProvider(@"D:\Music\Love radio"));
         

         Console.WriteLine("Available channels");
         foreach (ChannelInfo channelInfo in playerInfo.Player.GetChannels())
         {
            Console.WriteLine(channelInfo);
         }

         IPlaylistAudioPlayer playlistAudioPlayer = (IPlaylistAudioPlayer)playerInfo.Player;

         Console.WriteLine("Playlist");
         foreach (var sounds in playlistAudioPlayer.GetPlayList())
         {
            Console.WriteLine(sounds.Name);
         }

         playlistAudioPlayer.EnableChannel(0);          
         playlistAudioPlayer.Play();
         Console.ReadLine();


         

         playlistAudioPlayer.Play(playlistAudioPlayer.GetCurrentSoundIndex() + 1);
         controller.DeleteAudioPlayer(playerInfo.Id);
         Console.ReadLine();
         
         controller.Dispose();
         return;
         
         Console.ReadLine();
         playerInfo.Player.DisableChannel(0);
         Console.ReadLine();
         return;

         IEndpoint endpoint = new WCFEndpoint();
         endpoint.Start();
         Console.ReadLine();
         return;

         //AudioPlayerController controller = new AudioPlayerController();
         //AudioPlayerInfo playerInfo = controller.CreateAudioPlayer("My", new DirectorySoundProvider(@"D:\Music\VKMusic"));
         //playerInfo.Player.EnableChannel(0);
         //playerInfo.Player.Play();
         //Console.ReadLine();
         //playerInfo.Player.EnableChannel(0);
         //Console.ReadLine();
         //playerInfo.Player.DisableChannel(0);
         //WaveOutEvent @out = new WaveOutEvent();
         //Mp3FileReader fileReader = new Mp3FileReader(@"D:\Music\VKMusic\2 Chainz (feat. Wiz Khalifa) – We Own It.mp3");
         //Console.WriteLine(WaveOut.DeviceCount);
         //for (int i = 0; i < WaveOut.DeviceCount; i++)
         //{
         //    Console.WriteLine(WaveOut.GetCapabilities(i).ProductName);
         //}

         ////AudioProvider provider = new AudioProvider();

         ////provider.SeekTo(provider.Length - );

         //@out.NumberOfBuffers = 2;
         //@out.DesiredLatency = 100;
         //@out.Init(fileReader);
         //@out.AddWaveoutManager(0);
         //@out.Play();

         //Console.ReadLine();
         //@out.AddWaveoutManager(0);
         ////@out.AddWaveoutManager(1);



         ////WaveOutEvent @out2 = new WaveOutEvent();
         ////Mp3FileReader fileReader2 = new Mp3FileReader(@"D:\Music\Кристина\Ozon – Noma Numa Ye  D.mp3");
         ////@out2.NumberOfBuffers = 3;
         ////@out2.Init(fileReader2);
         ////@out2.AddWaveoutManager(0);


         ////@out.Play();
         ////Console.WriteLine(fileReader.Length);
         ////@out2.Play();
         //Console.ReadLine();
         //@out.Pause();



         //Console.ReadLine();
         //@out.Play();



         Console.ReadLine();

      }

      //static void PlayMp3(string url)
      //{
      //   BufferedWaveProvider bufferedWaveProvider;
      //   bool fullyDownloaded = false;
      //   WebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
      //   HttpWebResponse resp;
      //   resp = (HttpWebResponse)webRequest.GetResponse();
      //   var buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

      //   IMp3FrameDecompressor decompressor = null;
      //   try
      //   {
      //      using (var responseStream = resp.GetResponseStream())
      //      {
      //         var readFullyStream = new ReadFullyStream(responseStream);
      //         do
      //         {
      //               Mp3Frame frame;
      //               try
      //               {
      //                  frame = Mp3Frame.LoadFromStream(readFullyStream);                         
      //               }
      //               catch (EndOfStreamException)
      //               {
      //                  fullyDownloaded = true;
      //                  // reached the end of the MP3 file / stream
      //                  break;
      //               }
      //               catch (WebException)
      //               {
      //                  // probably we have aborted download from the GUI thread
      //                  break;
      //               }
      //               if (decompressor == null)
      //               {
      //                  // don't think these details matter too much - just help ACM select the right codec
      //                  // however, the buffered provider doesn't know what sample rate it is working at
      //                  // until we have a frame
      //                  decompressor = CreateFrameDecompressor(frame);
      //                  bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
      //                  bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
      //                  //this.bufferedWaveProvider.BufferedDuration = 250;
      //               }
      //               int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
      //               //Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
      //               bufferedWaveProvider.AddSamples(buffer, 0, decompressed);

      //         } while (playbackState != StreamingPlaybackState.Stopped);
      //         // was doing this in a finally block, but for some reason
      //         // we are hanging on response stream .Dispose so never get there
      //         decompressor.Dispose();
      //      }
      //   }
      //   finally
      //   {
      //      if (decompressor != null)
      //      {
      //         decompressor.Dispose();
      //      }
      //   }
      //}

      private static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
      {
         WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
             frame.FrameLength, frame.BitRate);
         return new AcmMp3FrameDecompressor(waveFormat);
      }
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alnet.AudioServer.Web.Models;
using Alnet.AudioServer.Web.AudioServerService;
using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAudioServerService _audioPlayerService = ApplicationContext.Instance.AudioPlayerService;

        public ActionResult Index()
        {
            AudioPlayerDTO[] audioPlayers = _audioPlayerService.GetAudioPlayes();
            MainPageModel model = new MainPageModel();
            foreach (var audioPlayer in audioPlayers)
            {
                PlaybackPositionDTO playbackPosition = audioPlayer.Type == PlayerTypes.Playlist ?
                    _audioPlayerService.GetPlaybackPosition(audioPlayer.Id)
                    : new PlaybackPositionDTO();

                ChannelDTO[] enabledChannels = _audioPlayerService.GetEnabledChannels(audioPlayer.Id);
                PlayerShortInfoModel playerShortInfo = new PlayerShortInfoModel()
                                                       {
                                                           PlayerId = audioPlayer.Id,
                                                           CurrentSoundName = playbackPosition.SoundName,
                                                           EnabledChannels = enabledChannels.Select(c => c.Name).ToList()
                                                       };
                model.Players.Add(playerShortInfo);
            }
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

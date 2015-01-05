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
        private readonly IAudioPlayerService _audioPlayerService = ApplicationContext.Instance.AudioPlayerService;

        public ActionResult Index()
        {
            PlaylistAudioPlayerDTO[] playlistAudioPlayers = _audioPlayerService.GetAudioPlayes();
            MainPageModel model = new MainPageModel
                                  {
                                      Players = new List<PlayerModel>()
                                  };
            foreach (var audioPlayer in playlistAudioPlayers)
            {
                model.Players.Add(PlayerModel.Parse(audioPlayer));
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

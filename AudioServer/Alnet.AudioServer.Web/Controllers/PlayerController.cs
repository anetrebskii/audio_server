using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Alnet.AudioServer.Web.AudioServerService;
using Alnet.AudioServer.Web.Models;
using Alnet.AudioServerContract.DTO;

namespace Alnet.AudioServer.Web.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IAudioPlayerService _audioPlayerService = ApplicationContext.Instance.AudioPlayerService;
        //
        // GET: /Player/

        public ActionResult View(Guid id)
        {
            PlaylistAudioPlayerDTO audioPlayer = _audioPlayerService.GetPlaylistAudioPlayer(id);
            return View(PlayerModel.Parse(audioPlayer));
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult NewVkPlayer(int id)
        {
            PlaylistAudioPlayerDTO newAudioPlayer = _audioPlayerService.CreateVKAudioPlayer("vk", id);
            return RedirectToAction("View", new { id = newAudioPlayer.Id });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alnet.AudioServer.Web.Models;

namespace Alnet.AudioServer.Web.Controllers
{
    public class PlayerController : Controller
    {
        //
        // GET: /Player/

        public ActionResult View(Guid id)
        {
            List<SoundModel> playList = new List<SoundModel>();
            playList.Add(new SoundModel()
            {
                Index = 0,
                Name = "Дискотека Авария"
            });
            playList.Add(new SoundModel()
            {
                Index = 1,
                Name = "Бандерос"
            });

            List<RoomModel> rooms = new List<RoomModel>();
            rooms.Add(new RoomModel()
            {
                Id = 1,
                IsSelected = true,
                Name = "Спальня"
            });
            rooms.Add(new RoomModel()
            {
                Id = 2,
                IsSelected = true,
                Name = "Кухня"
            });

            PlayerModel model = new PlayerModel()
            {
                PlayerId = id,
                Current = playList.First(),
                PlayList = playList,
                Rooms = rooms
            };
            return View(model);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(string vkId)
        {
            return RedirectToAction("View", new { id = Guid.NewGuid() });
        }
    }
}

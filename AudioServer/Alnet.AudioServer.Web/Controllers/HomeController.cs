using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alnet.AudioServer.Web.Models;

namespace Alnet.AudioServer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            MainPageModel model = new MainPageModel();
            model.Players = new List<PlayerModel>();
            model.Players.Add(new PlayerModel()
            {
                Current = new SoundModel()
                {
                    Index = 1,
                    Name = "Бандерос"
                },
                PlayerId = Guid.NewGuid()
            });
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

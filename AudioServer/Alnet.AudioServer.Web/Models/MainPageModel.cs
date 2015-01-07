using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alnet.AudioServer.Web.Models
{
    public class MainPageModel
    {
        public MainPageModel()
        {
            Players = new List<PlayerShortInfoModel>();
        }

        public List<PlayerShortInfoModel> Players { get; set; }
    }
}
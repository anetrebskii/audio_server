using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

using Alnet.AudioServer.Web.Components.AudioServerService;
using Alnet.AudioServer.Web.Models;
using Alnet.AudioServerContract;
using Alnet.AudioServerContract.DTO;

using IAudioServerService = Alnet.AudioServer.Web.AudioServerService.IAudioServerService;

namespace Alnet.AudioServer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAudioServerService _audioPlayerService = ApplicationContext.Instance.AudioPlayerService;

        public ActionResult Index()
        {
            try
            {
                AudioPlayerDTO[] audioPlayers = _audioPlayerService.GetAudioPlayes();                
                MainPageModel model = new MainPageModel();
                foreach (var audioPlayer in audioPlayers)
                {
                    PlaybackPositionDTO playbackPosition;
                    try
                    {
                        playbackPosition = audioPlayer.Type == PlayerTypes.Playlist ?
                            _audioPlayerService.GetPlaybackPosition(audioPlayer.Id)
                            : new PlaybackPositionDTO();
                    }
                    catch (FaultException<FaultCodes> ex)
                    {
                        if (ex.Detail == FaultCodes.NoSounds)
                        {
                            playbackPosition = new PlaybackPositionDTO();
                        }
                        else
                        {
                            throw;
                        }
                    }

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
            catch (ServiceException)
            {
                return RedirectToAction("Error", new {message = "Ну вот, у меня проблема с подключение к аудио серверу. Пожалуйста, скажи администратору.", returnUrl = Request.Url});
            }
            catch (FaultException<FaultCodes>)
            {

                return RedirectToAction("Error", new { message = "Возникла ошибка и я не знаю, что с ней делать. Пожалуйста, скажи администратору.", returnUrl = Request.Url});
            }
        }

        public ActionResult Error(string message, string returnUrl)
        {
            ViewBag.Message = message;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}

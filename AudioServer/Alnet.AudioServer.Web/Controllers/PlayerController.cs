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
    public class PlayerController : Controller
    {
        private readonly IAudioServerService _audioServerService = ApplicationContext.Instance.AudioPlayerService;
        //
        // GET: /Player/

        public ActionResult Stop(Guid id)
        {
            _audioServerService.Stop(id);
            return new EmptyResult();
        }

        public ActionResult GetPlaybackPositionJson(Guid id)
        {
            PlaybackPositionDTO playbackPosition = _audioServerService.GetPlaybackPosition(id);

            return Json(new PlaybackModel()
                        {
                            IsPlaying = playbackPosition.IsPlaying,
                            SoundName = playbackPosition.SoundName,
                            PlaybackPosition = playbackPosition.PlaybackPosition
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Play(Guid id)
        {
            _audioServerService.Play(id);
            return new EmptyResult();
        }

        public ActionResult PlayConcrete(Guid id, int otherId)
        {
            _audioServerService.PlayConcrete(id, otherId);
            return new EmptyResult();
        }

        public ActionResult MoveNextSound(Guid id)
        {
            _audioServerService.MoveNextSound(id);
            return new EmptyResult();
        }

        public ActionResult MovePrevSound(Guid id)
        {
            _audioServerService.MovePrevSound(id);
            return new EmptyResult();
        }

        public ActionResult EnableChannel(Guid id, int otherId)
        {
            _audioServerService.ChangeChannelState(id, otherId, true);
            return new EmptyResult();
        }

        public ActionResult DisableChannel(Guid id, int otherId)
        {
            _audioServerService.ChangeChannelState(id, otherId, false);
            return new EmptyResult();
        }

        public ActionResult View(Guid id)
        {
            try
            {
                AudioPlayerDTO audioPlayer = _audioServerService.GetAudioPlayer(id);
                if (audioPlayer.Type != PlayerTypes.Playlist)
                {
                    throw new NotSupportedException("Supported only playlist player");
                }
                SoundDTO[] sounds = _audioServerService.GetSounds(id);
                ChannelDTO[] allChannels = _audioServerService.GetAllChannels();
                ChannelDTO[] enabledChannels = _audioServerService.GetEnabledChannels(id);
                PlaybackPositionDTO playbackPosition = _audioServerService.GetPlaybackPosition(id);

                PlayerFullInfoModel playerInfo = new PlayerFullInfoModel()
                                                 {
                                                     PlayerId = audioPlayer.Id,
                                                     Playback = new PlaybackModel()
                                                                {
                                                                    IsPlaying = playbackPosition.IsPlaying,
                                                                    SoundName = playbackPosition.SoundName,
                                                                    PlaybackPosition = playbackPosition.PlaybackPosition
                                                                },
                                                     Sounds = sounds.Select(s => s.Name).ToList(),
                                                     Channels = allChannels.Select(c => new ChannelModel()
                                                                                        {
                                                                                            Name = c.Name,
                                                                                            IsEnabled = enabledChannels.Any(ec => ec.Index == c.Index)
                                                                                        }).ToList()
                                                 };
                return View(playerInfo);
            }
            catch (ServiceException)
            {
                return RedirectToAction("Error", "Home", new { message = "Ну вот, у меня проблема с подключение к аудио серверу. Пожалуйста, скажи администратору.", returnUrl = Request.Url});
            }
            catch (FaultException<FaultCodes>)
            {

                return RedirectToAction("Error", "Home", new { message = "Возникла ошибка и я не знаю, что с ней делать. Пожалуйста, скажи администратору.", returnUrl = Request.Url });
            }
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult NewVkPlayer(int id)
        {
            AudioPlayerDTO newAudioPlayer = _audioServerService.CreateVKAudioPlayer("vk", id);
            return RedirectToAction("View", new { id = newAudioPlayer.Id });
        }
    }
}

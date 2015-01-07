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
        private readonly IAudioServerService _audioServerService = ApplicationContext.Instance.AudioPlayerService;
        //
        // GET: /Player/

        public ActionResult Play(Guid id)
        {
            _audioServerService.Play(id);
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

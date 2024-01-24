using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Models;
using MusicStoreApi.Services;

namespace MusicStoreApi.Controllers
{
    [Route("api/song")]
    [ApiController]
    public class AllSongsController : ControllerBase
    {
        private readonly IAllSongsService allSongService;

        public AllSongsController(IAllSongsService allSongService)
        {
            this.allSongService = allSongService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<SongDto>> GetAll([FromQuery] SongQuery songQuery)
        {
            var songs = allSongService.GetAll(songQuery);

            return Ok(songs);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Models;
using MusicStoreApi.Services;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist/{artistId}/album")]
    [ApiController]
    [Authorize]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService albumService;

        public AlbumController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPost]
        public ActionResult Create([FromRoute] int artistId, [FromBody]CreateAlbumDto createAlbumDto)
        {
            int newAlbumId = albumService.Create(artistId, createAlbumDto);

            return Created($"/api/artist/{artistId}/album/{newAlbumId}", null);
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPut("{albumId}")]
        public ActionResult Update(int artistId, [FromRoute] int albumId, [FromBody]UpdateAlbumDto updateAlbumDto)
        {
            albumService.Update(artistId, albumId, updateAlbumDto);

            return Ok($"/api/artist/{artistId}/album/{albumId}");
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpDelete("{albumId}")]
        public ActionResult Delete([FromRoute]int artistId, [FromRoute] int albumId) 
        {
            albumService.Delete(artistId, albumId);

            return Ok($"/api/artist/{artistId}/album/{albumId}");
        }

        [HttpGet]
        public ActionResult<List<AlbumDto>> GetAll([FromRoute]int artistId) 
        {
            var albums = albumService.GetAll(artistId);

            return Ok(albums);
        }

        [HttpGet("{albumId}")]
        public ActionResult GetById([FromRoute] int artistId, [FromRoute]int albumId)
        {
            var album  = albumService.GetById(artistId ,albumId);

            return Ok(album);
        }
    }
}

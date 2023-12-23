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

        [HttpPost]
        public ActionResult Create([FromRoute] int artistId, [FromBody]CreateAlbumDto createAlbumDto)
        {
            int newAlbumId = albumService.Create(artistId, createAlbumDto);

            return Created($"/api/artist/{artistId}/album/{newAlbumId}", null);
        }

        [HttpPut("{albumId}")]
        public ActionResult Update(int artistId, [FromRoute] int albumId, [FromBody]UpdateAlbumDto updateAlbumDto)
        {
            albumService.Update(artistId, albumId, updateAlbumDto);

            return Ok($"/api/artist/{artistId}/album/{albumId}");
        }

        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int artistId)
        {
            albumService.DeleteAll(artistId);

            return Ok($"Deleted all albums: api/artist/{artistId}");
        }

        [HttpDelete("{albumId}")]
        public ActionResult DeleteById([FromRoute]int artistId, [FromRoute] int albumId) 
        {
            albumService.DeleteById(artistId, albumId);

            return Ok($"/api/artist/{artistId}/album/{albumId}");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<AlbumDto>> GetAll([FromRoute]int artistId, [FromQuery] AlbumQuery albumQuery) 
        {
            var albums = albumService.GetAll(artistId, albumQuery);

            return Ok(albums);
        }

        [AllowAnonymous]
        [HttpGet("{albumId}")]
        public ActionResult GetById([FromRoute] int artistId, [FromRoute]int albumId)
        {
            var album  = albumService.GetById(artistId ,albumId);

            return Ok(album);
        }
    }
}

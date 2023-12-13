using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Models;
using MusicStoreApi.Services;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist/{artistId}/album")]
    [ApiController]

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

            return Ok();
        }

        [HttpDelete("{albumId}")]
        public ActionResult Delete(int artistId, [FromRoute] int albumId) 
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
        public ActionResult GetById(int artistId, [FromRoute]int albumId)
        {
            var album  = albumService.GetById(artistId ,albumId);

            return Ok(album);
        }
    }
}

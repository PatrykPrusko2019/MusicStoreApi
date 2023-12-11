using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;
using MusicStoreApi.Services;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist")]
    [ApiController]
    public class ArtistController : ControllerBase
    {

        private readonly IArtistService artistService;

        public ArtistController(IArtistService artistService)
        {
            this.artistService = artistService;
        }

        [HttpPost]
        public ActionResult CreateArtist([FromBody] CreateArtistDto createdArtistDto)
        {
            int id = artistService.Create(createdArtistDto);

            return Created($"/api/artist/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteArtist([FromRoute] int id)
        {
            artistService.Delete(id);

            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute]int id, [FromBody]UpdateArtistDto updateArtistDto)
        {
            artistService.Update(id, updateArtistDto);

            return Ok();
        }


        [HttpGet]
        public ActionResult<IEnumerable<ArtistDto>> GetAll()
        {
            var artistsDtos = artistService.GetAll();

            return Ok(artistsDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<ArtistDto> Get([FromRoute] int id)
        {
            var artistDto = artistService.GetById(id);

            return Ok(artistDto);
        }

    }
}

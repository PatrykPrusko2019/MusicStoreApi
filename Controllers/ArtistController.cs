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
    public class ArtistController : ControllerBase
    {
        
        private readonly IArtistService artistService;

        public ArtistController(IArtistService artistService)
        {
            this.artistService = artistService;
        }

        [HttpPost]
        public ActionResult CreateArtist([FromBody]CreateArtistDto createdArtistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int id = artistService.Create(createdArtistDto);

            return Created($"/api/artist/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteArtist([FromRoute]int id) 
        {
            bool isDeleted = artistService.Delete(id);

            if (isDeleted) return NoContent();

            return NotFound();
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

            if (artistDto == null) return NotFound();

            return Ok(artistDto);
        }

    }
}

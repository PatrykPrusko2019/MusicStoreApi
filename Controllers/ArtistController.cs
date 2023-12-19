using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;
using MusicStoreApi.Services;
using System.Security.Claims;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist")]
    [ApiController]
    [Authorize]
    public class ArtistController : ControllerBase
    {

        private readonly IArtistService artistService;

        public ArtistController(IArtistService artistService)
        {
            this.artistService = artistService;
        }

        [HttpPost]
        public ActionResult Create([FromBody] CreateArtistDto createdArtistDto)
        {
            int id = artistService.Create(createdArtistDto);

            return Created($"/api/artist/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            artistService.Delete(id);

            return Ok($"/api/artist/{id}");
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute]int id, [FromBody]UpdateArtistDto updateArtistDto)
        {
            artistService.Update(id, updateArtistDto);

            return Ok($"/api/artist/{id}");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<ArtistDto>> GetAll([FromQuery]ArtistQuery artistQuery)
        {
            var artistsDtos = artistService.GetAll(artistQuery);

            return Ok(artistsDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<ArtistDto> Get([FromRoute] int id)
        
        {
            var artistDto = artistService.GetById(id);

            return Ok(artistDto);
        }

    }
}

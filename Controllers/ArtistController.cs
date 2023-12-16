using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPost]
        public ActionResult Create([FromBody] CreateArtistDto createdArtistDto)
        {
            int id = artistService.Create(createdArtistDto);

            return Created($"/api/artist/{id}", null);
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            artistService.Delete(id);

            return Ok($"/api/artist/{id}");
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPut("{id}")]
        public ActionResult Update([FromRoute]int id, [FromBody]UpdateArtistDto updateArtistDto)
        {
            artistService.Update(id, updateArtistDto);

            return Ok($"/api/artist/{id}");
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

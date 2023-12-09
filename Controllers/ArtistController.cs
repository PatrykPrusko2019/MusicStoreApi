using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistDbContext dbContext;

        public ArtistController(ArtistDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Artist>> GetAll(int id)
        {
            var artists = dbContext.Artists.ToList();

            return Ok(artists);
        }

        [HttpGet("{id}")]
        public ActionResult<Artist> Get([FromRoute] int id)
        {
            var artist = dbContext.Artists.FirstOrDefault(a => a.Id == id);

            return Ok(artist);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Models;
using MusicStoreApi.Services;

namespace MusicStoreApi.Controllers
{
    [Route("api/login/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("{email}")]
        public ActionResult GetUserByEmail([FromRoute] string email)
        {
            UserDto user = userService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpGet("{userId}/artist")]
        public ActionResult GetArtistsByUserId([FromRoute] int userId)
        {
            List<DetailsArtistDto> artists = userService.GetArtistsByUserId(userId);
            return Ok(artists);
        }
    }
}

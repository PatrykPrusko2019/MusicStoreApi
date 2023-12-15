using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicStoreApi.Models
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IAccountService
    {
        string GenerateJwt(LoginDto loginDto);
        void RegisterUser(RegisterUserDto registerUserDto);
    }
}

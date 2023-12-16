using MusicStoreApi.Models;
using System.Security.Claims;

namespace MusicStoreApi.Services
{
    public interface IArtistService
    {
        int Create(CreateArtistDto createdArtistDto, int userId);
        void Delete(int id, ClaimsPrincipal user);
        void Update(int id, UpdateArtistDto updatedArtistDto, ClaimsPrincipal user);
        List<ArtistDto> GetAll();
        ArtistDto GetById(int id);
    }
}
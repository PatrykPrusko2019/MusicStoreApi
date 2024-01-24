using MusicStoreApi.Models;
using System.Security.Claims;

namespace MusicStoreApi.Services
{
    public interface IArtistService
    {
        int Create(CreateArtistDto createdArtistDto);
        void Delete(int id);
        void Update(int id, UpdateArtistDto updatedArtistDto);
        PageResult<ArtistDto> GetAll(ArtistQuery searchQuery);
        ArtistDto GetById(int id);
        DetailsArtistDto GetDetailsById(int id);
    }
}
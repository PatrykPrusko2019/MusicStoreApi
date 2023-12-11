using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IArtistService
    {
        int Create(CreateArtistDto createdArtistDto);
        bool Delete(int id);
        bool Update(int id, UpdateArtistDto updatedArtistDto);
        IEnumerable<ArtistDto> GetAll();
        ArtistDto GetById(int id);
    }
}
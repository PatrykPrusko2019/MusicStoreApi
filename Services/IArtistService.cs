using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IArtistService
    {
        int Create(CreateArtistDto createdArtistDto);
        void Delete(int id);
        void Update(int id, UpdateArtistDto updatedArtistDto);
        IEnumerable<ArtistDto> GetAll();
        ArtistDto GetById(int id);
    }
}
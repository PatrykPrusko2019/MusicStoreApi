using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IAlbumService
    {
        int Create(int artistId, CreateAlbumDto createAlbumDto);
        void Update(int artistId, int albumId, UpdateAlbumDto updateAlbumDto);
        void DeleteAll(int artistId);
        void DeleteById(int artistId, int albumId);
        List<AlbumDto> GetAll(int artistId, AlbumQuery searchQuery);
        AlbumDto GetById(int artistId , int albumId);
    }
}
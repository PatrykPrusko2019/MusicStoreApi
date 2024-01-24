using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IAllAlbumsService
    {
        List<AlbumDto> GetAll(AlbumQuery searchQuery);
    }
}

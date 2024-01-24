using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface IAllSongsService
    {
        List<SongDto> GetAll(SongQuery songQuery);
    }
}

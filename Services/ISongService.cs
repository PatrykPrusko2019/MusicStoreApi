using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public interface ISongService
    {
        int Create(int artistId, int albumId, CreateSongDto songDto);
        void Update(int artistId, int albumId, int songId, UpdateSongDto createSongDto);
        void Delete(int artistId, int albumId, int songId);
        List<SongDto> GetAll(int artistId, int albumId);
        SongDto GetById(int artistId, int albumId, int songId);

    }

}
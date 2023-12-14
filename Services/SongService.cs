using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public class SongService : ISongService
    {
        private readonly ArtistDbContext artistDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<SongService> logger;

        public SongService(ArtistDbContext artistDbContext, IMapper mapper, ILogger<SongService> logger)
        {
            this.artistDbContext = artistDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        public int Create(int artistId, int albumId, CreateSongDto createSongDto)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album == null) { throw new NotFoundException("Artist, Album not found"); }

            var songEntity = mapper.Map<Song>(createSongDto);
            songEntity.AlbumId = album.Id;

            artistDbContext.Songs.Add(songEntity);
            artistDbContext.SaveChanges();
            logger.LogInformation($"Created new album: {songEntity.Name} , api/artist/{artistId}/album/{albumId}/song/{songEntity.Id}");

            return songEntity.Id;
        }

        public void Update(int artistId, int albumId, int songId, UpdateSongDto createSongDto)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId&& a.Id == albumId);
            if (album is null || album.Songs.IsNullOrEmpty()) throw new NotFoundException("Artist, Album, Song not found");

            var song = album.Songs.FirstOrDefault(s => s.Id == songId);
            if (song is null) throw new NotFoundException("Song not found");

            song.Name = createSongDto.Name;

            artistDbContext.SaveChanges();
            logger.LogInformation($"Updated song: {song.Name} , api/artist/{artistId}/album/{albumId}/song/{song.Id}");
        }

        public void Delete(int artistId, int albumId, int songId)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album is null || album.Songs.IsNullOrEmpty()) throw new NotFoundException("Artist, Album, Song not found");

            var deleteSong = album.Songs.FirstOrDefault(s => s.Id == songId);
            if (deleteSong is null) throw new NotFoundException("Song not found");

            string name = deleteSong.Name;
            artistDbContext.Songs.Remove(deleteSong);
            artistDbContext.SaveChanges();
            logger.LogInformation($"Removed song: {name} , api/artist/{artistId}/album/{albumId}/song/{songId}");
        }



        public List<SongDto> GetAll(int artistId, int albumId) 
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album is null || album.Songs.IsNullOrEmpty()) throw new NotFoundException("Artist, Album not found");

            var songsDtos = mapper.Map<List<SongDto>>(album.Songs);
            return songsDtos;
        }

        public SongDto GetById(int artistId, int albumId, int songId)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album is null || album.Songs.IsNullOrEmpty()) throw new NotFoundException("Artist, Album, Song not found");

            var song = album.Songs.FirstOrDefault(s => s.Id == songId);
            if (song is null) throw new NotFoundException("Song not found");

            var songDto = mapper.Map<SongDto>(song);
            return songDto;
        }


    }
}

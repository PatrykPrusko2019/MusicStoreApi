using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Linq.Expressions;

namespace MusicStoreApi.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ArtistService> logger;

        public AlbumService(ArtistDbContext dbContext, IMapper mapper, ILogger<ArtistService> logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

       
        public int Create(int artistId, CreateAlbumDto createAlbumDto)
        {
            GetArtistById(artistId);

            var albumEntity = mapper.Map<Album>(createAlbumDto);

            albumEntity.ArtistId = artistId;
            dbContext.Albums.Add(albumEntity);
            dbContext.SaveChanges();
            logger.LogInformation($"Created new album: {albumEntity.Title} , api/artist/{artistId}/album/{albumEntity.Id}");

            return albumEntity.Id;
        }

        public void Update(int artistId, int albumId, UpdateAlbumDto updateAlbumDto)
        {
            var album = GetAlbumById(artistId, albumId);

            album.Title = updateAlbumDto.Title;
            album.Length = updateAlbumDto.Length;
            album.NumberOfSongs = updateAlbumDto.NumberOfSongs;
            album.Price = updateAlbumDto.Price;

            dbContext.SaveChanges();
            logger.LogInformation($"Updated album: {album.Title} , api/artist/{artistId}/album/{album.Id}");
        }

        public void Delete(int artistId, int albumId)
        {
            var album = GetAlbumById(artistId, albumId);

            string title = album.Title;
            dbContext.Albums.Remove(album);

            dbContext.SaveChanges();
            logger.LogInformation($"Delete album: {title} , api/artist/{artistId}/album/{albumId}");
        }

        public List<AlbumDto> GetAll(int artistId)
        {
            GetArtistById(artistId);

            var albums = dbContext.Albums
                .Include(s => s.Songs)
                .Where(a => a.ArtistId == artistId).ToList();

            if (albums is null || albums.Count == 0) throw new NotFoundException("Album not found");

            var albumsDtos = mapper.Map<List<AlbumDto>>(albums);

            return albumsDtos;
        }

        public AlbumDto GetById(int artistId, int albumId)
        {
            var album = GetAlbumById(artistId, albumId);

            var albumDto = mapper.Map<AlbumDto>(album);

            return albumDto;
        }

        private Artist GetArtistById(int artistId)
        {
            var artist = dbContext.Artists.FirstOrDefault(a => a.Id == artistId);
            if (artist == null) throw new NotFoundException("Artist not found");
            return artist;
        }

        private Album GetAlbumById(int artistId, int albumId)
        {
            var album = dbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album is null) throw new NotFoundException("Artist and Album not found");
            return album;
        }
    }
}

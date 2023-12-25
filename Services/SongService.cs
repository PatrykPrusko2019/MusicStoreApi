using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Authorization;
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
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public SongService(ArtistDbContext artistDbContext, IMapper mapper, ILogger<SongService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.artistDbContext = artistDbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public int Create(int artistId, int albumId, CreateSongDto createSongDto)
        {
            CheckIfIdIsCorrectAndGetSongs(artistId, albumId, false);

            GetAuthorizationResult(artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Create);

            CheckIsUnigueName(artistId,  albumId, createSongDto.Name);

            var songEntity = mapper.Map<Song>(createSongDto);
            songEntity.AlbumId = albumId;

            CheckIsCorrectNumberOfSongs(albumId, 1);
            artistDbContext.Songs.Add(songEntity);
            artistDbContext.SaveChanges();

            logger.LogInformation($"Created new song: {songEntity.Name} , api/artist/{artistId}/album/{albumId}/song/{songEntity.Id}");
            return songEntity.Id;
        }

        public void Update(int artistId, int albumId, int songId, UpdateSongDto createSongDto)
        {
            var song = GetSongById(artistId, albumId, songId, false);

            GetAuthorizationResult(artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Update);

            CheckIsUnigueName(artistId, albumId, createSongDto.Name);

            song.Name = createSongDto.Name;

            artistDbContext.SaveChanges();
            logger.LogInformation($"Updated song: {song.Name} , api/artist/{artistId}/album/{albumId}/song/{song.Id}");
        }

        public void DeleteAll(int artistId, int albumId)
        {
            var deleteSongs = CheckIfIdIsCorrectAndGetSongs(artistId, albumId, true);

            GetAuthorizationResult(artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Delete);

            CheckIsCorrectNumberOfSongs(albumId, 0);
            foreach (var songs in deleteSongs) artistDbContext.Songs.Remove(songs);
            artistDbContext.SaveChanges();

            logger.LogInformation($"Removed song: api/artist/{artistId}/album/{albumId}");
        }

        public void DeleteById(int artistId, int albumId, int songId)
        {
            var deleteSong = GetSongById(artistId, albumId, songId, false);

            GetAuthorizationResult(artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Delete);

            CheckIsCorrectNumberOfSongs(albumId, -1);
            string name = deleteSong.Name;
            artistDbContext.Songs.Remove(deleteSong);
            artistDbContext.SaveChanges();

            logger.LogInformation($"Removed song: {name} , api/artist/{artistId}/album/{albumId}/song/{songId}");
        }

        public List<SongDto> GetAll(int artistId, int albumId) 
        {
            var songs = CheckIfIdIsCorrectAndGetSongs(artistId, albumId, true);

            var songsDtos = mapper.Map<List<SongDto>>(songs);
            return songsDtos;
        }

        public SongDto GetById(int artistId, int albumId, int songId)
        {
            var song = GetSongById(artistId, albumId, songId, false);

            var songDto = mapper.Map<SongDto>(song);
            return songDto;
        }

        private Song GetSongById(int artistId, int albumId, int songId, bool isGetSongsOrIsCheckId)
        {
            Artist artist = artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId);
            if (artist == null) throw new NotFoundException($"Artist {artistId} is not found");

            Album album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album == null) throw new NotFoundException($"Album {albumId} is not found");

            if (isGetSongsOrIsCheckId) return new Song();

            Song song = album.Songs.FirstOrDefault(s => s.AlbumId == albumId && s.Id == songId);
            
            if (song == null) throw new NotFoundException($"Song {songId} is not found");
                  
            return song;
        }

        private List<Song> CheckIfIdIsCorrectAndGetSongs(int artistId, int albumId, bool isGetSongsOrIsCheckId)
        {
            GetSongById(artistId, albumId, 0, true); // checks if ids numbers are correct

            if (isGetSongsOrIsCheckId)
            {
                var album = artistDbContext.Albums
                    .Include (s => s.Songs)
                    .FirstOrDefault(a => a.Id == albumId);
                if (album.Songs.IsNullOrEmpty()) throw new NotFoundException("list of songs is empty");
                else return album.Songs;
            }

            return null;
        }

        private void CheckIsCorrectNumberOfSongs(int albumId, int counter)
        {
            Album album = artistDbContext.Albums.Include(s => s.Songs).FirstOrDefault(a => a.Id == albumId);
            if (counter != 0) album.NumberOfSongs = album.Songs.Count + counter;
            else album.NumberOfSongs = 0;
        }

        private void GetAuthorizationResult(Artist deleteArtist, ResourceOperation delete)
        {
            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, deleteArtist, new ResourceOperationRequirement(delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
        }

        private void CheckIsUnigueName(int artistId, int albumId, string name)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);

            if (album.Songs.IsNullOrEmpty()) return;
            var isDuplicate = album.Songs.Any(a => a.Name == name);
            if (isDuplicate) throw new DuplicateValueException("Name : value invalid, because is on the songs's list");
        }

    }
}

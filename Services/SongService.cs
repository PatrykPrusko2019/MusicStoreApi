using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Authorization;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Linq.Expressions;

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

            CheckIsUniqueName(artistId, albumId, createSongDto.Name, -1);

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

            CheckIsUniqueName(artistId, albumId, createSongDto.Name, songId);

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

            logger.LogInformation($"Removed song: api/artist/{artistId}/album/{albumId}/song");
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

        public List<SongDto> GetAll(int artistId, int albumId, SongQuery searchQuery)
        {
            CheckIfIdIsCorrectAndGetSongs(artistId, albumId, true);

            var baseQuery = artistDbContext.Songs
                .Where(s => s.AlbumId == albumId)
                .Where(s => searchQuery.SearchWord == null || s.Name.ToLower().Contains(searchQuery.SearchWord.ToLower()));

            if (!string.IsNullOrEmpty(searchQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Song, object>>>
                {
                    { nameof(Song.Name), a => a.Name }
                };

                var selectedColumn = columnsSelectors[searchQuery.SortBy];

                baseQuery = searchQuery.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var songsDtos = mapper.Map<List<SongDto>>(baseQuery);
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

        public DetailsSongDto GetDetailsById(int artistId, int albumId, int songId)
        {
            var song = GetById(artistId, albumId, songId);

            string nameArtist = artistDbContext.Artists.FirstOrDefault(a => a.Id == artistId).Name;

            string nameAlbum = artistDbContext.Albums.FirstOrDefault(a => a.Id == albumId).Title;

            DetailsSongDto detailsSongDto = new DetailsSongDto()
            {
                Id = song.Id,
                Name = song.Name,
                AlbumId = albumId,
                AlbumTitle = nameAlbum,
                ArtistId = artistId,
                ArtistName = nameArtist
            };

            return detailsSongDto;
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

        private void CheckIsUniqueName(int artistId, int albumId, string name, int songId)
        {
            var album = artistDbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);

            if (album.Songs.IsNullOrEmpty()) return;

            bool isDuplicate = false;

            if (songId != -1) //update value
            {
                var result = album.Songs.Any(a => a.Name == name);
                if (result)
                {
                    var songIdDuplicate = album.Songs.FirstOrDefault(a => a.Name == name).Id;
                    if (songIdDuplicate != songId) isDuplicate = true;
                }
            }
            else //create value
            {
                isDuplicate = album.Songs.Any(a => a.Name == name);
            }

            if (isDuplicate) throw new DuplicateValueException("Name : value invalid, because is on the songs's list");
        }

    }
}

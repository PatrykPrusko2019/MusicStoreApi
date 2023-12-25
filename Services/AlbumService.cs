using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Authorization;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Linq;
using System.Linq.Expressions;

namespace MusicStoreApi.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ArtistService> logger;
        private readonly IUserContextService userContextService;
        private readonly IAuthorizationService authorizationService;

        public AlbumService(ArtistDbContext dbContext, IMapper mapper, ILogger<ArtistService> logger, IUserContextService userContextService, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.userContextService = userContextService;
            this.authorizationService = authorizationService;    
        }

       
        public int Create(int artistId, CreateAlbumDto createAlbumDto)
        {
            CheckIfIdIsCorrectAndGetAlbums(artistId, false);

            GetAuthorizationResult(dbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Create);

            CheckIsUnigueTitle(artistId, createAlbumDto.Title);

            var albumEntity = mapper.Map<Album>(createAlbumDto);

            albumEntity.ArtistId = artistId;
            dbContext.Albums.Add(albumEntity);
            dbContext.SaveChanges();
            logger.LogInformation($"Created new album: {albumEntity.Title} , api/artist/{artistId}/album/{albumEntity.Id}");

            return albumEntity.Id;
        }

        public void Update(int artistId, int albumId, UpdateAlbumDto updateAlbumDto)
        {
            var album = GetAlbumById(artistId, albumId, false);

            GetAuthorizationResult(dbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Update);

            CheckIsUnigueTitle(artistId, updateAlbumDto.Title);

            album.Title = updateAlbumDto.Title;
            album.Length = updateAlbumDto.Length;
            album.Price = updateAlbumDto.Price;

            dbContext.SaveChanges();
            logger.LogInformation($"Updated album: {album.Title} , api/artist/{artistId}/album/{album.Id}");
        }

        public void DeleteAll(int artistId)
        {
            var removeAlbums = CheckIfIdIsCorrectAndGetAlbums(artistId, true);

            GetAuthorizationResult(dbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Delete);

            foreach (var album in removeAlbums) dbContext.Albums.Remove(album);
            
            dbContext.SaveChanges();
            logger.LogInformation($"Deleted all albums: api/artist/{artistId}");
        }

        public void DeleteById(int artistId, int albumId)
        {
            var album = GetAlbumById(artistId, albumId, false);

            GetAuthorizationResult(dbContext.Artists.FirstOrDefault(a => a.Id == artistId), ResourceOperation.Delete);

            string title = album.Title;
            dbContext.Albums.Remove(album);

            dbContext.SaveChanges();
            logger.LogInformation($"Deleted album: {title} , api/artist/{artistId}/album/{albumId}");
        }

        public List<AlbumDto> GetAll(int artistId, AlbumQuery searchQuery)
        {
            var albums = CheckIfIdIsCorrectAndGetAlbums(artistId, true);

            var baseQuery = dbContext.Albums
                .Include(a => a.Songs)
                .Where(a => a.ArtistId == artistId)
                .Where(a => searchQuery.SearchWord == null || a.Title.ToLower().Contains(searchQuery.SearchWord.ToLower()));

            if (!string.IsNullOrEmpty(searchQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Album, object>>>
                {
                    { nameof(Album.Title), a => a.Title }
                };

                var selectedColumn = columnsSelectors[searchQuery.SortBy];

                baseQuery = searchQuery.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var albumsDtos = mapper.Map<List<AlbumDto>>(baseQuery);

            return albumsDtos;
        }

        public AlbumDto GetById(int artistId, int albumId)
        {
            var album = GetAlbumById(artistId, albumId, false);

            var albumDto = mapper.Map<AlbumDto>(album);

            return albumDto;
        }

        private Album GetAlbumById(int artistId, int albumId, bool isGetSongsOrIsCheckId)
        {
            Artist artist = dbContext.Artists.FirstOrDefault(a => a.Id == artistId);
            if (artist == null) throw new NotFoundException($"Artist {artistId} is not found");

            if (isGetSongsOrIsCheckId) return null;

            Album album = dbContext.Albums
                .Include(s => s.Songs)
                .FirstOrDefault(a => a.ArtistId == artistId && a.Id == albumId);
            if (album == null) throw new NotFoundException($"Album {albumId} is not found");

            return album;
        }

        private List<Album> CheckIfIdIsCorrectAndGetAlbums(int artistId,  bool isGetAlbumsOrIsCheckId)
        {
            GetAlbumById(artistId, 0, true); // checks if ids numbers are correct

            if (isGetAlbumsOrIsCheckId)
            {
                var albums = dbContext.Albums
                    .Include(s => s.Songs)
                    .Where(a => a.ArtistId == artistId).ToList();
                if (albums.IsNullOrEmpty()) throw new NotFoundException("list of albums is empty");
                else return albums;
            }

            return null;
        }

        private void GetAuthorizationResult(Artist deleteArtist, ResourceOperation delete)
        {
            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, deleteArtist, new ResourceOperationRequirement(delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
        }

        private void CheckIsUnigueTitle(int artistId, string title)
        {
            var artist = dbContext.Artists
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.Id == artistId);

            if (artist.Albums.IsNullOrEmpty()) return;

            var isDuplicate = artist.Albums.Any(a => a.Title == title);
            if (isDuplicate) throw new DuplicateValueException("Title : value invalid, because is on the album's list");
        }


    }
}

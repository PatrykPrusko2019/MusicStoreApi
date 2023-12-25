using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Authorization;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace MusicStoreApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ArtistService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public ArtistService(ArtistDbContext dbContext, IMapper mapper, ILogger<ArtistService> logger, IAuthorizationService authorizationService, 
            IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
                
        }

        public int Create(CreateArtistDto createdArtistDto)
        {
            CheckIsUnigueName(createdArtistDto.Name);

            var createdArtist = mapper.Map<Artist>(createdArtistDto);
            createdArtist.CreatedById = userContextService.GetUserId;

            GetAuthorizationResult(createdArtist, ResourceOperation.Create);

            dbContext.Artists.Add(createdArtist);
            dbContext.SaveChanges();
            logger.LogInformation($"Created new artist: {createdArtist.Name} , api/artist/{createdArtist.Id}");

            return createdArtist.Id;
        }

        public void Delete(int id)
        {
            var deleteArtist = GetArtistById(id);

            string name = deleteArtist.Name;

            GetAuthorizationResult(deleteArtist, ResourceOperation.Delete);

            dbContext.Artists.Remove(deleteArtist);
            dbContext.SaveChanges();
            logger.LogInformation($"Deleted artist: {name} , api/artist/{id}");

        }

        public PageResult<ArtistDto> GetAll(ArtistQuery searchQuery)
        {
            // .Skip(searchQuery.PageSize * (searchQuery.PageNumber - 1)) -> 5 * (2 - 1) = 10 -> skip 10 items
            var baseQuery = dbContext.Artists
                .Include(a => a.Address)
                .Include(a => a.Albums)
                .Where(a => searchQuery.SearchWord == null || (a.Name.ToLower().Contains(searchQuery.SearchWord.ToLower())
                                               || a.Description.ToLower().Contains(searchQuery.SearchWord.ToLower())));

            if (!string.IsNullOrEmpty(searchQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Artist, object>>>
                {
                    { nameof(Artist.Name), a => a.Name },
                    { nameof(Artist.Description), a => a.Description },
                    { nameof(Artist.KindOfMusic), a => a.KindOfMusic },
                };

                var selectedColumn = columnsSelectors[searchQuery.SortBy];

                baseQuery = searchQuery.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var artists = baseQuery
                .Skip(searchQuery.PageSize * (searchQuery.PageNumber - 1))
                .Take(searchQuery.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            if (totalItemsCount <= searchQuery.PageSize * (searchQuery.PageNumber - 1))
            {

                throw new BadRequestException($"search result items of Artists: {totalItemsCount} is too small or equal, because the number of skip: {searchQuery.PageSize * (searchQuery.PageNumber - 1)} "
                                              + ", change the values in 'PageSize = 5, PageNumber = 1' , to see the result");
            }

            if (artists is null || artists.Count == 0) throw new NotFoundException(searchQuery.SearchWord == null ? "list of artists is empty" : $"searchWord not found: {searchQuery.SearchWord}");

            artists.ForEach(artist =>
            {
                int count = 0; 
                while (artist.Albums.Count > count)
                {
                    var searchSongs = dbContext.Songs.Where(s => s.AlbumId == artist.Albums[count].Id).ToList();
                    artist.Albums[count++].Songs = searchSongs;
                }
            });

            var artistsDtos = mapper.Map<List<ArtistDto>>(artists);

            var result = new PageResult<ArtistDto>(artistsDtos, totalItemsCount, searchQuery.PageSize, searchQuery.PageNumber);

            return result;
        }

        public ArtistDto GetById(int id)
        {
            var artist = GetArtistById(id);

            if (artist.Albums is not null)
            {
                int count = 0;
                while (artist.Albums.Count > count)
                {
                    var searchSongs = dbContext.Songs.Where(s => s.AlbumId == artist.Albums[count].Id).ToList();
                    artist.Albums[count++].Songs = searchSongs;
                }
            }

            var artistDto = mapper.Map<ArtistDto>(artist);

            return artistDto;
        }

        public void Update(int id, UpdateArtistDto updatedArtistDto) 
        {
            var artist = GetArtistById(id);

            artist.Address = dbContext.Addresses.FirstOrDefault(a => a.Id == id);

            GetAuthorizationResult(artist, ResourceOperation.Update);

            CheckIsUnigueName(updatedArtistDto.Name);

            artist.Name = updatedArtistDto.Name;
            artist.Description = updatedArtistDto.Description;
            artist.KindOfMusic = updatedArtistDto.KindOfMusic;
            artist.ContactEmail = updatedArtistDto.ContactEmail;
            artist.ContactNumber = updatedArtistDto.ContactNumber;
            artist.Address.Country = updatedArtistDto.Country;
            artist.Address.City = updatedArtistDto.City;

            dbContext.SaveChanges();
            logger.LogInformation($"Updated artist: {artist.Name} , api/artist/{artist.Id}");
        }

        private Artist GetArtistById(int artistId)
        {
            Artist artist = dbContext.Artists
                .Include(a => a.Address)
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.Id == artistId);
            if (artist is null) throw new NotFoundException($"Artist {artistId} is not found");
            return artist;
        }

        private void GetAuthorizationResult(Artist deleteArtist, ResourceOperation delete)
        {
            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, deleteArtist, new ResourceOperationRequirement(delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
        }

        private void CheckIsUnigueName(string name)
        {
            var userId = int.Parse( userContextService.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value );
            
            if (! dbContext.Artists.IsNullOrEmpty()) 
            {
                var isDuplicate = dbContext.Artists.Any(a => a.Name == name && a.CreatedById == userId);
                if (isDuplicate) throw new DuplicateValueException("Name: invalid value because there is already an artist created by this user (duplicate)");
            }
            
        }
    }
}

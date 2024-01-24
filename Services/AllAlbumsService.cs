using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Linq.Expressions;

namespace MusicStoreApi.Services
{
    public class AllAlbumsService : IAllAlbumsService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;

        public AllAlbumsService(ArtistDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public List<AlbumDto> GetAll(AlbumQuery searchQuery)
        {
            var albums = dbContext.Albums;
            if (albums.IsNullOrEmpty()) throw new NotFoundException("list of albums is empty");

            var baseQuery = dbContext.Albums
                .Include(a => a.Songs)
                .Where(a => searchQuery.SearchWord == null || a.Title.ToLower().Contains(searchQuery.SearchWord.ToLower()));

            if (!string.IsNullOrEmpty(searchQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Album, object>>>
                {
                    { nameof(Album.Title), a => a.Title }
                };

                var selectedColumn = columnsSelectors[searchQuery.SortBy];

                switch (searchQuery.SortDirection)
                {
                    case SortDirection.ASC:
                        baseQuery = baseQuery.OrderBy(selectedColumn);
                        break;
                    case SortDirection.DESC:
                        baseQuery = baseQuery.OrderByDescending(selectedColumn);
                        break;
                }
            }

            var albumsDtos = mapper.Map<List<AlbumDto>>(baseQuery);

            return albumsDtos;
        }

    }
}

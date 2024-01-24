using AutoMapper;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;
using System.Linq.Expressions;

namespace MusicStoreApi.Services
{
    public class AllSongsService : IAllSongsService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;

        public AllSongsService(ArtistDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public List<SongDto> GetAll(SongQuery songQuery)
        {
            var baseQuery = dbContext.Songs
                .Where(s => songQuery.SearchWord == null || s.Name.ToLower().Contains(songQuery.SearchWord.ToLower()));

            if (!string.IsNullOrEmpty(songQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Song, object>>>
                {
                    { nameof(Song.Name), a => a.Name }
                };

                var selectedColumn = columnsSelectors[songQuery.SortBy];

                switch (songQuery.SortDirection)
                {
                    case SortDirection.ASC:
                        baseQuery = baseQuery.OrderBy(selectedColumn);
                        break;
                    case SortDirection.DESC:
                        baseQuery = baseQuery.OrderByDescending(selectedColumn);
                        break;
                }
            }

            var songsDtos = mapper.Map<List<SongDto>>(baseQuery);
            return songsDtos;
        }
    }
}

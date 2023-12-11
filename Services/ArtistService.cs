using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;

        public ArtistService(ArtistDbContext artistDbContext, IMapper mapper)
        {
            this.dbContext = artistDbContext;
            this.mapper = mapper;
        }

        public int Create(CreateArtistDto createdArtistDto)
        {
            var createdArtist = mapper.Map<Artist>(createdArtistDto);
            dbContext.Artists.Add(createdArtist);
            dbContext.SaveChanges();

            return createdArtist.Id;
        }

        public bool Delete(int id)
        {
            var deleteArtist = dbContext.Artists.FirstOrDefault(a => a.Id == id);

            if (deleteArtist is null) return false;

            dbContext.Artists.Remove(deleteArtist);
            dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ArtistDto> GetAll()
        {
            var artists = dbContext.Artists
                .Include(a => a.Address)
                .Include(a => a.Albums)
                .ToList();

            if (artists is null || artists.Count == 0) return null;

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

            return artistsDtos;
        }

        public ArtistDto GetById(int id)
        {
            var artist = dbContext.Artists
                .Include(a => a.Address)
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.Id == id);

            if (artist is null)
            {
                return null;
            }

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

        public bool Update(int id, UpdateArtistDto updatedArtistDto)
        {
            var artist = dbContext.Artists.FirstOrDefault(a => a.Id == id);
            if (artist is null) { return  false; }

            artist.Address = dbContext.Addresses.FirstOrDefault(a => a.Id == id);

            artist.Name = updatedArtistDto.Name;
            artist.Description = updatedArtistDto.Description;
            artist.KindOfMusic = updatedArtistDto.KindOfMusic;
            artist.ContactEmail = updatedArtistDto.ContactEmail;
            artist.ContactNumber = updatedArtistDto.ContactNumber;
            artist.Address.Country = updatedArtistDto.Country;
            artist.Address.City = updatedArtistDto.City;

            dbContext.SaveChanges();

            return true;
        }
    }
}

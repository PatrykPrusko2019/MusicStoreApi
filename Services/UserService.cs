using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;

namespace MusicStoreApi.Services
{
    public class UserService : IUserService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;

        public UserService(ArtistDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public UserDto GetUserByEmail(string email)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Email == email);
            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }

        public List<DetailsArtistDto> GetArtistsByUserId(int id)
        {
            var user = GetUserById(id);

            var artists = dbContext.Artists
                .Include(a => a.Albums)
                .Include(a => a.Address)
                .Where(a => a.CreatedById == user.Id).ToArray();

            int count = 0;
            foreach (var artist in artists)
            {
                while (artist.Albums.Count > count)
                {
                    var searchSongs = dbContext.Songs.Where(s => s.AlbumId == artist.Albums[count].Id).ToList();
                    artist.Albums[count++].Songs = searchSongs;
                }
            }

            var artistsDto = mapper.Map<List<DetailsArtistDto>>(artists);
            return artistsDto;
        }

        public UserDto GetUserById(int id)
        {
            var user = dbContext.Users
                .FirstOrDefault(u => u.Id == id);

            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }

    }
}

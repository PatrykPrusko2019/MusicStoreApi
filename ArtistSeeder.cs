using Microsoft.EntityFrameworkCore;
using MusicStoreApi.Entities;

namespace MusicStoreApi
{
    public class ArtistSeeder
    {
        private readonly ArtistDbContext _dbContext;
        private List<string> NamesArtists = new List<string>() { "Iron Maiden", "Offspring", "Korn", "Metallica", "ACDC", "Dzem", "Sepultura", "Green Day", "Vergin", "Venflon"};


        public ArtistSeeder(ArtistDbContext artistDbContext)
        {
            _dbContext = artistDbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if(pendingMigrations != null && pendingMigrations.Any() )
                {
                    _dbContext.Database.Migrate();
                }
            }

            if (!_dbContext.Roles.Any()) 
            {
                var roles = GetRoles();
                _dbContext.Roles.AddRange(roles);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Users.Any())
            {
                var users = GetUsers();
                _dbContext.Users.AddRange(users);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Artists.Any())
            {
                var artists = GetArtists();
                _dbContext.Artists.AddRange(artists);
                _dbContext.SaveChanges();
            }

        }

        private IEnumerable<Role> GetRoles()
        {
            var rules = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "PremiumUser"
                },
                new Role()
                {
                    Name = "Admin"
                }
            };
            return rules;
        }

        private IEnumerable<Artist> GetArtists()
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == "premiumuser@gmail.com");

            var artists = new List<Artist>();
            int count = 1;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    var artist = new Artist()
                    {
                        Name = $"{NamesArtists[i]} {count}",
                        Description = $"information about ... {count}",
                        KindOfMusic = $"rock, metal {count}",
                        ContactEmail = $"contact@email{count}.com",
                        ContactNumber = $"98742385{count}",
                        CreatedById = user.Id,
                        Albums = new List<Album>()
                        {
                            new Album()
                            {
                                Title = $"Armagedon {count}",
                                Length = 3.5,
                                NumberOfSongs = 3,
                                Price = 10.30,
                                Songs = new List<Song>()
                                {
                                    new Song()
                                    {
                                        Name = $"Planet is good 1 {count}"
                                    },
                                    new Song()
                                    {
                                        Name = $"This is a cat 2 {count}"
                                    },
                                    new Song()
                                    {
                                        Name = $"So tired 3 {count}"
                                    }
                                }

                            },
                            new Album()
                            {
                                Title = $"Dance to death {count}",
                                Length = 3.5,
                                NumberOfSongs = 3,
                                Price = 10.30,
                                Songs = new List<Song>()
                                {
                                    new Song()
                                    {
                                        Name = $"Where are you baby 1 {count}"
                                    },
                                    new Song()
                                    {
                                        Name = $"Alicia is nice 2 {count}"
                                    },
                                    new Song()
                                    {
                                        Name = $"I'm crying 3 {count}"
                                    }
                                }

                            }

                        },
                        Address = new Address()
                        {
                            Country = $"USA {count}",
                            City = $"New York {count}"
                        }

                    };
                    artists.Add(artist);
                    count++;
                }
                
            }
            return artists;
        }

        private IEnumerable<User> GetUsers()
        {
            var users = new List<User>()
            {
                new User() 
                {
                    Email = "user@gmail.com",
                    FirstName = "Adam",
                    LastName = "Ciska",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Nationality = "Polish",
                    PasswordHash = "AQAAAAIAAYagAAAAELZ0WsgVpKqiOuq0p71A3FElecFHLyovnucnYIZo3rtCOPzD6LV1jjuxiT/IPBg3oQ==",
                    RoleId = 1
                },
                new User()
                {
                    Email = "premiumuser@gmail.com",
                    FirstName = "Blazej",
                    LastName = "Mana",
                    DateOfBirth = new DateTime(1987, 5, 6),
                    Nationality = "Polish",
                    PasswordHash = "AQAAAAIAAYagAAAAEF6eiowEI+NHdELuR43kc5QmPeyAcjAtT7bwCdIPZrntS1XLjefrLaCHttbz62mLWg==",
                    RoleId = 2
                },
                new User()
                {
                    Email = "admin@gmail.com",
                    FirstName = "Grzegorz",
                    LastName = "Rybka",
                    DateOfBirth = new DateTime(2000, 2, 3),
                    Nationality = "Greek",
                    PasswordHash = "AQAAAAIAAYagAAAAEIzlKkobOYvA14lhndufA7pNJVUdpnxRMbhu4mrp+Y91tH+wyNLhDFJKTX494+JGlA==",
                    RoleId = 3
                }
            };
            return users;
        }


    }
}

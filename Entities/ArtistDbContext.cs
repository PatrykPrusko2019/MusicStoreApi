using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace MusicStoreApi.Entities
{
    public class ArtistDbContext : DbContext
    {
        public ArtistDbContext(DbContextOptions<ArtistDbContext> options) : base(options) { }

        public DbSet<Artist> Artists { get; set;}
        public DbSet<Album> Albums { get; set;}
        public DbSet<Song>  Songs { get; set;}
        public DbSet<Address> Addresses { get; set;}
        public DbSet<User> Users { get; set;}
        public DbSet<Role>  Roles { get; set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(25);
            modelBuilder.Entity<Artist>()
                .Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Album>()
                .Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Song>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Address>()
                .Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(a => a.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(a => a.FirstName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(a => a.LastName)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();

        }
    }
}

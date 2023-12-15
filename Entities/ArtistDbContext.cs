using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MusicStoreApi.Entities
{
    public class ArtistDbContext : DbContext
    {
        private string _connectionString = "Server=DESKTOP-09DCRPN;Database=MusicStoreDb;Trusted_Connection=True;TrustServerCertificate=True";
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

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}

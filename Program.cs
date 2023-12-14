

using MusicStoreApi;
using MusicStoreApi.Entities;
using MusicStoreApi.Middleware;
using MusicStoreApi.Services;
using NLog.Web;
using System.Reflection;

public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // NLog: Setup Nlog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ArtistDbContext>();
            builder.Services.AddScoped<ArtistSeeder>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddScoped<IArtistService, ArtistService>();
            builder.Services.AddScoped<IAlbumService, AlbumService>();
            builder.Services.AddScoped<ISongService, SongService>();
            builder.Services.AddScoped<ErrorHandlingMiddleware>();
            builder.Services.AddScoped<RequestTimeMiddleware>();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<ArtistSeeder>();
            seeder.Seed();

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicStore API");
            });        

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

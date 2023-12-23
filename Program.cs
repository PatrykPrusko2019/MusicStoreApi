using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Text;
using FluentValidation.AspNetCore;
using System.Reflection;
using MusicStoreApi;
using MusicStoreApi.Entities;
using MusicStoreApi.Authorization;
using MusicStoreApi.Services;
using MusicStoreApi.Models;
using MusicStoreApi.Models.Validators;
using MusicStoreApi.Middleware;
using Microsoft.EntityFrameworkCore;


public class Program
    {
    public static void Main(string[] args)
        {
        var builder = WebApplication.CreateBuilder(args);

        // NLog: Setup Nlog for Dependency injection
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        // Add services to the container.

        var authenticationSettings = new AuthenticationSettings();
        builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

        builder.Services.AddSingleton(authenticationSettings);

        builder.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authenticationSettings.JwtIssuer,
                ValidAudience = authenticationSettings.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
            };
        });

        builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
        builder.Services.AddControllers().AddFluentValidation();
        builder.Services.AddScoped<ArtistSeeder>();
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        builder.Services.AddScoped<IArtistService, ArtistService>();
        builder.Services.AddScoped<IAlbumService, AlbumService>();
        builder.Services.AddScoped<ISongService, SongService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        builder.Services.AddScoped<IValidator<LoginDto>, LoginUserDtoValidator>();
        builder.Services.AddScoped<IValidator<ArtistQuery>, ArtistQueryValidator>();
        builder.Services.AddScoped<IValidator<AlbumQuery>, AlbumQueryValidator>();

        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        builder.Services.AddScoped<RequestTimeMiddleware>();
        builder.Services.AddScoped<IUserContextService, UserContextService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontEndClient", policyBuilder =>

                policyBuilder.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(builder.Configuration["AllowedOrigins"])
                );
        });

        builder.Services.AddDbContext<ArtistDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ArtistDbContext")));

        // Configure the HTTP request pipeline.
        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ArtistSeeder>();

        app.UseResponseCaching();
        app.UseStaticFiles();
        app.UseCors("FrontEndClient");

        seeder.Seed();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestTimeMiddleware>();
        app.UseAuthentication();
        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicStore API");
            // c.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    }

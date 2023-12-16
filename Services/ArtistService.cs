﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MusicStoreApi.Authorization;
using MusicStoreApi.Entities;
using MusicStoreApi.Exceptions;
using MusicStoreApi.Models;
using System.Security.Claims;

namespace MusicStoreApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ArtistDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ArtistService> logger;
        private readonly IAuthorizationService authorizationService;

        public ArtistService(ArtistDbContext dbContext, IMapper mapper, ILogger<ArtistService> logger, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
        }

        public int Create(CreateArtistDto createdArtistDto, int userId)
        {
            var createdArtist = mapper.Map<Artist>(createdArtistDto);
            createdArtist.CreatedById = userId;
            dbContext.Artists.Add(createdArtist);
            dbContext.SaveChanges();
            logger.LogInformation($"Created new artist: {createdArtist.Name} , api/artist/{createdArtist.Id}");

            return createdArtist.Id;
        }

        public void Delete(int id, ClaimsPrincipal user)
        {
            var deleteArtist = GetArtistById(id);

            string name = deleteArtist.Name;

            var authorizationResult = authorizationService.AuthorizeAsync(user, deleteArtist, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Artists.Remove(deleteArtist);
            dbContext.SaveChanges();
            logger.LogInformation($"Deleted artist: {name} , api/artist/{id}");

        }

        public List<ArtistDto> GetAll()
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

        public void Update(int id, UpdateArtistDto updatedArtistDto, ClaimsPrincipal user) 
        {
            var artist = GetArtistById(id);

            artist.Address = dbContext.Addresses.FirstOrDefault(a => a.Id == id);

            var authorizationResult = authorizationService.AuthorizeAsync(user, artist, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

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
            if (artist is null) throw new NotFoundException("Artist not found");
            return artist;
        }
    }
}

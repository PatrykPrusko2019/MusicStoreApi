using AutoMapper;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;

namespace MusicStoreApi
{
    public class ArtistMappingProfile : Profile
    {
        public ArtistMappingProfile()
        {
            CreateMap<Artist, ArtistDto>()
                .ForMember(m => m.City, c => c.MapFrom(a => a.Address.City))
                .ForMember(m => m.Country, c => c.MapFrom(a => a.Address.Country));

            CreateMap<Album, AlbumDto>();
            
            CreateMap<Song, SongDto>();

            CreateMap<CreateArtistDto, Artist>()
                .ForMember(a => a.Address, c => c.MapFrom(dto =>
                new Address() { Country = dto.Country, City = dto.City }));

            CreateMap<CreateAlbumDto, Album>();

            CreateMap<CreateSongDto, Song>();

        }
    }
}

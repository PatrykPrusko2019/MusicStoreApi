using MusicStoreApi.Entities;

namespace MusicStoreApi.Models
{
    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KindOfMusic { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public List<AlbumDto> Albums { get; set; }
        public int CreatedById { get; set; }
    }
}

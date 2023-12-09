using MusicStoreApi.Entities;

namespace MusicStoreApi.Models
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Length { get; set; }
        public int NumberOfSongs { get; set; }
        public double Price { get; set; }
        public List<SongDto> Songs { get; set; }
    }
}

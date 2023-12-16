using System.ComponentModel.DataAnnotations;

namespace MusicStoreApi.Models.ChangeAlbum
{
    public abstract class ChangeAlbumDto
    {
        [Required]
        [MaxLength(25)]
        public string Title { get; set; }
        public double Length { get; set; }
        public double Price { get; set; }
    }
}

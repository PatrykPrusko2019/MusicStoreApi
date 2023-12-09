using System.ComponentModel.DataAnnotations;

namespace MusicStoreApi.Models
{
    public class CreateArtistDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }
        public string KindOfMusic { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Country { get; set; }

        [Required]
        [MaxLength(20)]
        public string City { get; set; }
    }
}

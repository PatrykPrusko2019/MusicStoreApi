using System.ComponentModel.DataAnnotations;

namespace MusicStoreApi.Models.Change
{
    public abstract class ChangeArtistDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }
        public string KindOfMusic { get; set; }

        [EmailAddress]
        public string ContactEmail { get; set; }

        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Country { get; set; }

        [Required]
        [MaxLength(20)]
        public string City { get; set; }
    }
}

using MusicStoreApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace MusicStoreApi.Models.ChangeSong
{
    public abstract class ChangeSongDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
    }
}

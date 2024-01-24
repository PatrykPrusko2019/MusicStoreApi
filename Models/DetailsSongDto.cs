namespace MusicStoreApi.Models
{
    public class DetailsSongDto : SongDto
    {
        public string AlbumTitle { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
    }
}

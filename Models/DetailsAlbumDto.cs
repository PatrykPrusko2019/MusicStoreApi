namespace MusicStoreApi.Models
{
    public class DetailsAlbumDto : AlbumDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
    }
}

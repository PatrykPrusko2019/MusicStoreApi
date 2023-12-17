namespace MusicStoreApi.Models
{
    public class AlbumQuery 
    {
        public string SearchWord { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}

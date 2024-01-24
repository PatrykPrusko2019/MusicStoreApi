namespace MusicStoreApi.Models
{
    public class SongQuery
    {
        public string SearchWord { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}

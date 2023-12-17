namespace MusicStoreApi.Models
{
    public class ArtistQuery
    {
        public string SearchWord { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }

}

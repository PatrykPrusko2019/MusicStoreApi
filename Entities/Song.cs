namespace MusicStoreApi.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}

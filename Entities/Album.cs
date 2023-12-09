namespace MusicStoreApi.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Length { get; set; }
        public int NumberOfSongs { get; set; }
        public double Price { get; set; }    
            

        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual List<Song> Songs { get; set; }

    }
}

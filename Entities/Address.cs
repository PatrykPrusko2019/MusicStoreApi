namespace MusicStoreApi.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public virtual Artist Artist { get; set; }
    }
}

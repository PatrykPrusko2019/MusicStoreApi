﻿namespace MusicStoreApi.Entities
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KindOfMusic { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; }
        public virtual List<Album> Albums { get; set; }
    }
}

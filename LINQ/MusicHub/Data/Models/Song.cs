namespace MusicHub.Data.Models
{
    using System;
    using Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Song
    {
        public Song()
        {
            this.SongPerformers = new HashSet<SongPerformer>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreatedOn { get; set; }
        public Genre Genre { get; set; }
        public int? AlbumId { get; set; }
        public Album Album { get; set; }
        public int WriterId { get; set; }
        public Writer Writer { get; set; }
        public decimal Price { get; set; }

        public ICollection<SongPerformer> SongPerformers { get; set; }
    }
}

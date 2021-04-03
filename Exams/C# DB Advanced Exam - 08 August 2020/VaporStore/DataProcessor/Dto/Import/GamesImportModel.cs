﻿namespace VaporStore.DataProcessor.Import.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GamesImportModel
    {
        [Required]
        public string Name { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Price { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Genre { get; set; }
        public string[] Tags { get; set; }
    }
}
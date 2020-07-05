namespace CarbonKitchen.Api.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        [Required]
        [Column("RecipeId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int RecipeId { get; set; }

        [Column("Title")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Title { get; set; }

        [Column("Directions")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Directions { get; set; }

        [Column("RecipeSourceLink")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string RecipeSourceLink { get; set; }

        [Column("Description")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [Column("ImageLink")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string ImageLink { get; set; }
    }
}

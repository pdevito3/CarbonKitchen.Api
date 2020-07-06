namespace CarbonKitchen.Api.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    [Table("Ingredients")]
    public class Ingredient
    {
        [Key]
        [Required]
        [Column("IngredientId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int IngredientId { get; set; }

        [Column("RecipeId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int? RecipeId { get; set; }

        [Column("Name")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Column("Unit")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Unit { get; set; }

        [Column("Amount")]
        public double? Amount { get; set; }
    }
}

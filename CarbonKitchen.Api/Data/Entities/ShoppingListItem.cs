namespace CarbonKitchen.Api.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    [Table("ShoppingListItems")]
    public class ShoppingListItem
    {
        [Key]
        [Required]
        [Column("ShoppingListItemId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int ShoppingListItemId { get; set; }

        [Column("ShoppingListId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int? ShoppingListId { get; set; }

        [Column("Name")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Column("Category")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Category { get; set; }

        [Column("Amount")]
        [Sieve(CanFilter = true, CanSort = true)]
        public float? Amount { get; set; }

        [Column("Unit")]
        public DateTime? Unit { get; set; }

        [Column("Acquired")]
        [Sieve(CanFilter = true, CanSort = true)]
        public bool? Acquired { get; set; } = false;

        [Column("Hidden")]
        [Sieve(CanFilter = true, CanSort = true)]
        public bool? Hidden { get; set; } = false;
    }
}

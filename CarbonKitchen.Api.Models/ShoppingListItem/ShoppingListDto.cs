namespace CarbonKitchen.Api.Models.ShoppingListItem
{
    using System;

    public class ShoppingListItemDto
    {
        public int ShoppingListItemId { get; set; }
        public double? Amount { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public bool? Acquired { get; set; } = false;
        public bool? Hidden { get; set; } = false;
        public int? ShoppingListId { get; set; }
    }
}

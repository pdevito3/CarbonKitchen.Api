namespace CarbonKitchen.Api.Models.Ingredient
{
    using System;

    public class IngredientDto
    {
        public int IngredientId { get; set; }
        public int? RecipeId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double? Amount { get; set; }
    }
}

namespace CarbonKitchen.Api.Models.Recipe
{
    using System;

    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Directions { get; set; }
        public string RecipeSourceLink { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
    }
}

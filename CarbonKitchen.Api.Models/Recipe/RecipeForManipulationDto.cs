namespace CarbonKitchen.Api.Models.Recipe
{
    using System;

    public class RecipeForManipulationDto
    {
        public int? RecipeIntField1 { get; set; }
        public string Title { get; set; }
        public string Directions { get; set; }
        public string RecipeSourceLink { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public DateTime? RecipeDateField1 { get; set; }
    }
}

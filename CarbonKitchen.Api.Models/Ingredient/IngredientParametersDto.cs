namespace CarbonKitchen.Api.Models.Ingredient
{
    using CarbonKitchen.Api.Models.Pagination;

    public class IngredientParametersDto : IngredientPaginationParameters
    {
        public string Filters { get; set; }
        public string QueryString { get; set; }
        public string SortOrder { get; set; }
        public int? RecipeId { get; set; }
    }
}

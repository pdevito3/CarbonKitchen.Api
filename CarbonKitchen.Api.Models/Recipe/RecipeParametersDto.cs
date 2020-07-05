namespace CarbonKitchen.Api.Models.Recipe
{
    using CarbonKitchen.Api.Models.Pagination;

    public class RecipeParametersDto : RecipePaginationParameters
    {
        public string Filters { get; set; }
        public string QueryString { get; set; }
        public string SortOrder { get; set; }
    }
}

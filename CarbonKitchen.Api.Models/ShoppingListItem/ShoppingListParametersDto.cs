namespace CarbonKitchen.Api.Models.ShoppingListItem
{
    using CarbonKitchen.Api.Models.Pagination;

    public class ShoppingListItemParametersDto : ShoppingListItemPaginationParameters
    {
        public string Filters { get; set; }
        public string QueryString { get; set; }
        public string SortOrder { get; set; }
    }
}

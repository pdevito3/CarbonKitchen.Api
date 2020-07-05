namespace CarbonKitchen.Api.Services.ShoppingListItem
{
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using System.Threading.Tasks;

    public interface IShoppingListItemRepository
    {
        PagedList<ShoppingListItem> GetShoppingListItems(ShoppingListItemParametersDto ShoppingListItemParameters);
        Task<ShoppingListItem> GetShoppingListItemAsync(int ShoppingListItemId);
        ShoppingListItem GetShoppingListItem(int ShoppingListItemId);
        void AddShoppingListItem(ShoppingListItem ShoppingListItem);
        void DeleteShoppingListItem(ShoppingListItem ShoppingListItem);
        void UpdateShoppingListItem(ShoppingListItem ShoppingListItem);
        bool Save();
    }
}

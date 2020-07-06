namespace CarbonKitchen.Api.Services.ShoppingListItem
{
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ShoppingListItemRepository : IShoppingListItemRepository
    {
        private CarbonKitchenDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public ShoppingListItemRepository(CarbonKitchenDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<ShoppingListItem> GetShoppingListItems(ShoppingListItemParametersDto ShoppingListItemParameters)
        {
            if (ShoppingListItemParameters == null)
            {
                throw new ArgumentNullException(nameof(ShoppingListItemParameters));
            }

            var collection = _context.ShoppingListItems as IQueryable<ShoppingListItem>;

            if (!string.IsNullOrWhiteSpace(ShoppingListItemParameters.QueryString))
            {
                var QueryString = ShoppingListItemParameters.QueryString.Trim();
                collection = collection.Where(i => i.Name.Contains(QueryString)
                    || i.Category.Contains(QueryString));
            }

            var sieveModel = new SieveModel
            {
                Sorts = ShoppingListItemParameters.SortOrder,
                Filters = ShoppingListItemParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<ShoppingListItem>.Create(collection,
                ShoppingListItemParameters.PageNumber,
                ShoppingListItemParameters.PageSize);
        }

        public async Task<ShoppingListItem> GetShoppingListItemAsync(int ShoppingListItemId)
        {
            return await _context.ShoppingListItems.FirstOrDefaultAsync(i => i.ShoppingListItemId == ShoppingListItemId);
        }

        public ShoppingListItem GetShoppingListItem(int ShoppingListItemId)
        {
            return _context.ShoppingListItems.FirstOrDefault(i => i.ShoppingListItemId == ShoppingListItemId);
        }

        public void AddShoppingListItem(ShoppingListItem ShoppingListItem)
        {
            if (ShoppingListItem == null)
            {
                throw new ArgumentNullException(nameof(ShoppingListItem));
            }

            _context.ShoppingListItems.Add(ShoppingListItem);
        }

        public void DeleteShoppingListItem(ShoppingListItem ShoppingListItem)
        {
            if (ShoppingListItem == null)
            {
                throw new ArgumentNullException(nameof(ShoppingListItem));
            }

            _context.ShoppingListItems.Remove(ShoppingListItem);
        }

        public void UpdateShoppingListItem(ShoppingListItem ShoppingListItem)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

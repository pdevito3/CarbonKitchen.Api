namespace CarbonKitchen.Api.Data
{
    using CarbonKitchen.Api.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class ShoppingListItemDbContext : DbContext
    {
        public ShoppingListItemDbContext(DbContextOptions<ShoppingListItemDbContext> options) : base(options) { }

        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    }
}

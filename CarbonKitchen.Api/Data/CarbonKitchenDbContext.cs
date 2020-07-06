namespace CarbonKitchen.Api.Data
{
    using CarbonKitchen.Api.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class CarbonKitchenDbContext : DbContext
    {
        public CarbonKitchenDbContext(DbContextOptions<CarbonKitchenDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    }
}

﻿namespace CarbonKitchen.Api.Data
{
    using CarbonKitchen.Api.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
    }
}

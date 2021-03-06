﻿namespace CarbonKitchen.Api.Services.Recipe
{
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.Recipe;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RecipeRepository : IRecipeRepository
    {
        private CarbonKitchenDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public RecipeRepository(CarbonKitchenDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Recipe> GetRecipes(RecipeParametersDto recipeParameters)
        {
            if (recipeParameters == null)
            {
                throw new ArgumentNullException(nameof(recipeParameters));
            }

            var collection = _context.Recipes as IQueryable<Recipe>;

            if (!string.IsNullOrWhiteSpace(recipeParameters.QueryString))
            {
                var QueryString = recipeParameters.QueryString.Trim();
                collection = collection.Where(r => r.Title.Contains(QueryString)
                    || r.Directions.Contains(QueryString));
            }

            var sieveModel = new SieveModel
            {
                Sorts = recipeParameters.SortOrder,
                Filters = recipeParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Recipe>.Create(collection,
                recipeParameters.PageNumber,
                recipeParameters.PageSize);
        }

        public async Task<Recipe> GetRecipeAsync(int recipeId)
        {
            return await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId);
        }

        public Recipe GetRecipe(int recipeId)
        {
            return _context.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
        }

        public void AddRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            _context.Recipes.Add(recipe);
        }

        public void DeleteRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            _context.Recipes.Remove(recipe);
        }

        public void UpdateRecipe(Recipe recipe)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

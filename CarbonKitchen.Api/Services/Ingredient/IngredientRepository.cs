namespace CarbonKitchen.Api.Services.Ingredient
{
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models;
    using CarbonKitchen.Api.Models.Ingredient;
    using CarbonKitchen.Api.Models.Pagination;
    using Microsoft.AspNetCore.Razor.Language.Intermediate;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class IngredientRepository : IIngredientRepository
    {
        private CarbonKitchenDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public IngredientRepository(CarbonKitchenDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Ingredient> GetIngredients(IngredientParametersDto ingredientParameters)
        {
            if (ingredientParameters == null)
            {
                throw new ArgumentNullException(nameof(ingredientParameters));
            }

            var collection = _context.Ingredients as IQueryable<Ingredient>;

            if (!string.IsNullOrWhiteSpace(ingredientParameters.QueryString))
            {
                var QueryString = ingredientParameters.QueryString.Trim();
                collection = collection.Where(i => i.Name.Contains(QueryString)
                    || i.Unit.Contains(QueryString));
            }

            if (!string.IsNullOrWhiteSpace(ingredientParameters.RecipeId.ToString()))
            {
                collection = collection.Where(i => i.RecipeId.Equals(ingredientParameters.RecipeId));
            }

            var sieveModel = new SieveModel
            {
                Sorts = ingredientParameters.SortOrder,
                Filters = ingredientParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Ingredient>.Create(collection,
                ingredientParameters.PageNumber,
                ingredientParameters.PageSize);
        }

        public async Task<Ingredient> GetIngredientAsync(int ingredientId)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == ingredientId);
        }

        public Ingredient GetIngredient(int ingredientId)
        {
            return _context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
        }

        public void AddIngredient(Ingredient ingredient)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            _context.Ingredients.Add(ingredient);
        }
        public void AddIngredients(List<Ingredient> ingredients)
        {
            if (ingredients == null)
            {
                throw new ArgumentNullException(nameof(ingredients));
            }

            _context.Ingredients.AddRange(ingredients);
        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            _context.Ingredients.Remove(ingredient);
        }

        public void DeleteIngredients(List<Ingredient> ingredients)
        {
            _context.Ingredients.RemoveRange(ingredients);
        }

        //TODO: Update to get htis working and use this instead of list
        public void DeleteIngredients(int recipeId)
        {
            var ingredients = _context.Ingredients.Where(i => i.RecipeId == recipeId).ToList<Ingredient>();
            _context.Ingredients.RemoveRange(ingredients);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

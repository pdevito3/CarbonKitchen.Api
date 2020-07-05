namespace CarbonKitchen.Api.Services.Recipe
{
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.Recipe;
    using System.Threading.Tasks;

    public interface IRecipeRepository
    {
        PagedList<Recipe> GetRecipes(RecipeParametersDto recipeParameters);
        Task<Recipe> GetRecipeAsync(int recipeId);
        Recipe GetRecipe(int recipeId);
        void AddRecipe(Recipe recipe);
        void DeleteRecipe(Recipe recipe);
        void UpdateRecipe(Recipe recipe);
        bool Save();
    }
}

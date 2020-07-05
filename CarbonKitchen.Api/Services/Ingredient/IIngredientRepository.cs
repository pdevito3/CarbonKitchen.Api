namespace CarbonKitchen.Api.Services.Ingredient
{
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.Ingredient;
    using System.Threading.Tasks;

    public interface IIngredientRepository
    {
        PagedList<Ingredient> GetIngredients(IngredientParametersDto ingredientParameters);
        Task<Ingredient> GetIngredientAsync(int ingredientId);
        Ingredient GetIngredient(int ingredientId);
        void AddIngredient(Ingredient ingredient);
        void DeleteIngredient(Ingredient ingredient);
        void UpdateIngredient(Ingredient ingredient);
        bool Save();
    }
}

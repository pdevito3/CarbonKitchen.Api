namespace CarbonKitchen.Api.Tests.Fakes.Recipe
{
    using AutoBogus;
    using CarbonKitchen.Api.Models.Recipe;

    // or replace 'AutoFaker' with 'Faker' if you don't want all fields to be auto faked
    public class FakeRecipeDto : AutoFaker<RecipeDto>
    {
        public FakeRecipeDto()
        {
            // leaving the first 49 for potential special use cases in startup builds that need explicit values
            RuleFor(r => r.RecipeId, r => r.Random.Number(50, 100000));
        }
    }
}

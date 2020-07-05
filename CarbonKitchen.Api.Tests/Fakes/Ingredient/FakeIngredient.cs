namespace CarbonKitchen.Api.Tests.Fakes.Ingredient
{
    using AutoBogus;
    using CarbonKitchen.Api.Data.Entities;

    // or replace 'AutoFaker' with 'Faker' if you don't want all fields to be auto faked
    public class FakeIngredient : AutoFaker<Ingredient>
    {
        public FakeIngredient()
        {
            // leaving the first 49 for potential special use cases in startup builds that need explicit values
            RuleFor(i => i.IngredientId, i => i.Random.Number(50, 100000));
        }
    }
}

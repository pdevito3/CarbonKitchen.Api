namespace CarbonKitchen.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.Ingredient;
    using CarbonKitchen.Api.Tests.Fakes.Ingredient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using Xunit;

    public class CreateIngredientRepositoryTests
    {
        [Fact]
        public void AddIngredient_NewRecordAddedWithProperValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredient = new FakeIngredient { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                service.AddIngredient(fakeIngredient);

                context.SaveChanges();
            }

            //Assert
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Ingredients.Count().Should().Be(1);

                var ingredientById = context.Ingredients.FirstOrDefault(i => i.IngredientId == fakeIngredient.IngredientId);

                ingredientById.Should().BeEquivalentTo(fakeIngredient);
                ingredientById.IngredientId.Should().Be(fakeIngredient.IngredientId);
                ingredientById.Name.Should().Be(fakeIngredient.Name);
                ingredientById.Unit.Should().Be(fakeIngredient.Unit);
                ingredientById.Amount.Should().Be(fakeIngredient.Amount);
            }
        }
    }
}

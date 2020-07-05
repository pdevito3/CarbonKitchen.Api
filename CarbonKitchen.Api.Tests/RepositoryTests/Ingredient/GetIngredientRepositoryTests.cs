namespace CarbonKitchen.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Models.Ingredient;
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

    public class GetIngredientRepositoryTests
    {
        [Fact]
        public void GetIngredient_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredient = new FakeIngredient { }.Generate();

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredient);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var ingredientById = context.Ingredients.FirstOrDefault(i => i.IngredientId == fakeIngredient.IngredientId);

                ingredientById.Should().BeEquivalentTo(fakeIngredient);
                ingredientById.IngredientId.Should().Be(fakeIngredient.IngredientId);
                ingredientById.Name.Should().Be(fakeIngredient.Name);
                ingredientById.Unit.Should().Be(fakeIngredient.Unit);
                ingredientById.Amount.Should().Be(fakeIngredient.Amount);
            }
        }

        [Fact]
        public void GetIngredients_CountMatchesAndContainsEvuivalentObjects()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            var fakeIngredientThree = new FakeIngredient { }.Generate();

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto());

                //Assert
                ingredientRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientOne);
                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientTwo);
                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetIngredients_ReturnExpectedPageSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            var fakeIngredientThree = new FakeIngredient { }.Generate();

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { PageSize = 2 });

                //Assert
                ingredientRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientOne);
                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetIngredients_ReturnExpectedPageNumberAndSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            var fakeIngredientThree = new FakeIngredient { }.Generate();

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                ingredientRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                ingredientRepo.Should().ContainEquivalentOf(fakeIngredientTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetIngredients_ListSortedInAscOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            fakeIngredientOne.Name = "Bravo";

            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            fakeIngredientTwo.Name = "Alpha";

            var fakeIngredientThree = new FakeIngredient { }.Generate();
            fakeIngredientThree.Name = "Charlie";

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { SortOrder = "Name" });

                //Assert
                ingredientRepo.Should()
                    .ContainInOrder(fakeIngredientTwo, fakeIngredientOne, fakeIngredientThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetIngredients_ListSortedInDescOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            fakeIngredientOne.Name = "Bravo";

            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            fakeIngredientTwo.Name = "Alpha";

            var fakeIngredientThree = new FakeIngredient { }.Generate();
            fakeIngredientThree.Name = "Charlie";

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { SortOrder = "-Name" });

                //Assert
                ingredientRepo.Should()
                    .ContainInOrder(fakeIngredientThree, fakeIngredientOne, fakeIngredientTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Name == Alpha")]
        [InlineData("Unit == Bravo")]
        [InlineData("RecipeId == 5")]
        [InlineData("Name == Charlie")]
        [InlineData("Unit == Delta")]
        [InlineData("RecipeId == 6")]
        [InlineData("Name == Echo")]
        [InlineData("Unit == Foxtrot")]
        [InlineData("RecipeId == 7")]
        public void GetIngredients_FilterListWithExact(string filters)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            fakeIngredientOne.Name = "Alpha";
            fakeIngredientOne.Unit = "Bravo";
            fakeIngredientOne.RecipeId = 5;

            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            fakeIngredientTwo.Name = "Charlie";
            fakeIngredientTwo.Unit = "Delta";
            fakeIngredientTwo.RecipeId = 6;

            var fakeIngredientThree = new FakeIngredient { }.Generate();
            fakeIngredientThree.Name = "Echo";
            fakeIngredientThree.Unit = "Foxtrot";
            fakeIngredientThree.RecipeId = 7;

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { Filters = filters });

                //Assert
                ingredientRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Name@=Hart", 1)]
        [InlineData("Unit@=Fav", 1)]
        [InlineData("Name@=*hart", 2)]
        [InlineData("Unit@=*fav", 2)]
        public void GetIngredients_FilterListWithContains(string filters, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            fakeIngredientOne.Name = "Alpha";
            fakeIngredientOne.Unit = "Bravo";

            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            fakeIngredientTwo.Name = "Hartsfield";
            fakeIngredientTwo.Unit = "Favaro";

            var fakeIngredientThree = new FakeIngredient { }.Generate();
            fakeIngredientThree.Name = "Bravehart";
            fakeIngredientThree.Unit = "Jonfav";

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { Filters = filters });

                //Assert
                ingredientRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("hart", 1)]
        [InlineData("fav", 1)]
        [InlineData("Fav", 0)]
        public void GetIngredients_SearchQueryReturnsExpectedRecordCount(string queryString, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<IngredientDbContext>()
                .UseInMemoryDatabase(databaseName: $"IngredientDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeIngredientOne = new FakeIngredient { }.Generate();
            fakeIngredientOne.Name = "Alpha";
            fakeIngredientOne.Unit = "Bravo";

            var fakeIngredientTwo = new FakeIngredient { }.Generate();
            fakeIngredientTwo.Name = "Hartsfield";
            fakeIngredientTwo.Unit = "White";

            var fakeIngredientThree = new FakeIngredient { }.Generate();
            fakeIngredientThree.Name = "Bravehart";
            fakeIngredientThree.Unit = "Jonfav";

            //Act
            using (var context = new IngredientDbContext(dbOptions))
            {
                context.Ingredients.AddRange(fakeIngredientOne, fakeIngredientTwo, fakeIngredientThree);
                context.SaveChanges();

                var service = new IngredientRepository(context, new SieveProcessor(sieveOptions));

                var ingredientRepo = service.GetIngredients(new IngredientParametersDto { QueryString = queryString });

                //Assert
                ingredientRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }
    }
}

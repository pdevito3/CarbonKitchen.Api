namespace CarbonKitchen.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Models.Recipe;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.Recipe;
    using CarbonKitchen.Api.Tests.Fakes.Recipe;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using Xunit;

    public class GetRecipeRepositoryTests
    {
        [Fact]
        public void GetRecipe_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipe = new FakeRecipe { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipe);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var recipeById = context.Recipes.FirstOrDefault(r => r.RecipeId == fakeRecipe.RecipeId);

                recipeById.Should().BeEquivalentTo(fakeRecipe);
                recipeById.RecipeId.Should().Be(fakeRecipe.RecipeId);
                recipeById.Title.Should().Be(fakeRecipe.Title);
                recipeById.Directions.Should().Be(fakeRecipe.Directions);
            }
        }

        [Fact]
        public void GetRecipes_CountMatchesAndContainsEvuivalentObjects()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            var fakeRecipeThree = new FakeRecipe { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto());

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeOne);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ReturnExpectedPageSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            var fakeRecipeThree = new FakeRecipe { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { PageSize = 2 });

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeOne);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ReturnExpectedPageNumberAndSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            var fakeRecipeThree = new FakeRecipe { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ListSortedInAscOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.Title = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.Title = "Alpha";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.Title = "Charlie";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { SortOrder = "Title" });

                //Assert
                recipeRepo.Should()
                    .ContainInOrder(fakeRecipeTwo, fakeRecipeOne, fakeRecipeThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ListSortedInDescOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.Title = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.Title = "Alpha";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.Title = "Charlie";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { SortOrder = "-Title" });

                //Assert
                recipeRepo.Should()
                    .ContainInOrder(fakeRecipeThree, fakeRecipeOne, fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Title == Alpha")]
        [InlineData("Directions == Bravo")]
        [InlineData("Title == Charlie")]
        [InlineData("Directions == Delta")]
        [InlineData("Title == Echo")]
        [InlineData("Directions == Foxtrot")]
        public void GetRecipes_FilterListWithExact(string filters)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.Title = "Alpha";
            fakeRecipeOne.Directions = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.Title = "Charlie";
            fakeRecipeTwo.Directions = "Delta";
            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.Title = "Echo";
            fakeRecipeThree.Directions = "Foxtrot";
            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { Filters = filters });

                //Assert
                recipeRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Title@=Hart", 1)]
        [InlineData("Directions@=Fav", 1)]
        [InlineData("Title@=*hart", 2)]
        [InlineData("Directions@=*fav", 2)]
        public void GetRecipes_FilterListWithContains(string filters, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.Title = "Alpha";
            fakeRecipeOne.Directions = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.Title = "Hartsfield";
            fakeRecipeTwo.Directions = "Favaro";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.Title = "Bravehart";
            fakeRecipeThree.Directions = "Jonfav";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { Filters = filters });

                //Assert
                recipeRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("hart", 1)]
        [InlineData("fav", 1)]
        [InlineData("Fav", 0)]
        public void GetRecipes_SearchQueryReturnsExpectedRecordCount(string queryString, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.Title = "Alpha";
            fakeRecipeOne.Directions = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.Title = "Hartsfield";
            fakeRecipeTwo.Directions = "White";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.Title = "Bravehart";
            fakeRecipeThree.Directions = "Jonfav";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { QueryString = queryString });

                //Assert
                recipeRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }
    }
}

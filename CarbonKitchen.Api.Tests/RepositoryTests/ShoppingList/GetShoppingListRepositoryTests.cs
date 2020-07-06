namespace CarbonKitchen.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.ShoppingListItem;
    using CarbonKitchen.Api.Tests.Fakes.ShoppingListItem;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using Xunit;

    public class GetShoppingListItemRepositoryTests
    {
        [Fact]
        public void GetShoppingListItem_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItem = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItem);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var ShoppingListItemById = context.ShoppingListItems.FirstOrDefault(i => i.ShoppingListItemId == fakeShoppingListItem.ShoppingListItemId);

                ShoppingListItemById.Should().BeEquivalentTo(fakeShoppingListItem);
                ShoppingListItemById.ShoppingListItemId.Should().Be(fakeShoppingListItem.ShoppingListItemId);
                ShoppingListItemById.Name.Should().Be(fakeShoppingListItem.Name);
                ShoppingListItemById.Category.Should().Be(fakeShoppingListItem.Category);
                ShoppingListItemById.Unit.Should().Be(fakeShoppingListItem.Unit);
            }
        }

        [Fact]
        public void GetShoppingListItems_CountMatchesAndContainsEvuivalentObjects()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto());

                //Assert
                ShoppingListItemRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemOne);
                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemTwo);
                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetShoppingListItems_ReturnExpectedPageSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { PageSize = 2 });

                //Assert
                ShoppingListItemRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemOne);
                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetShoppingListItems_ReturnExpectedPageNumberAndSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                ShoppingListItemRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                ShoppingListItemRepo.Should().ContainEquivalentOf(fakeShoppingListItemTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetShoppingListItems_ListSortedInAscOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemOne.Name = "Bravo";

            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemTwo.Name = "Alpha";

            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemThree.Name = "Charlie";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { SortOrder = "Name" });

                //Assert
                ShoppingListItemRepo.Should()
                    .ContainInOrder(fakeShoppingListItemTwo, fakeShoppingListItemOne, fakeShoppingListItemThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetShoppingListItems_ListSortedInDescOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemOne.Name = "Bravo";

            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemTwo.Name = "Alpha";

            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemThree.Name = "Charlie";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { SortOrder = "-Name" });

                //Assert
                ShoppingListItemRepo.Should()
                    .ContainInOrder(fakeShoppingListItemThree, fakeShoppingListItemOne, fakeShoppingListItemTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Name == Alpha")]
        [InlineData("Category == Bravo")]
        [InlineData("ShoppingListId == 5")]
        [InlineData("Name == Charlie")]
        [InlineData("Category == Delta")]
        [InlineData("ShoppingListId == 6")]
        [InlineData("Name == Echo")]
        [InlineData("Category == Foxtrot")]
        [InlineData("ShoppingListId == 7")]
        public void GetShoppingListItems_FilterListWithExact(string filters)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemOne.Name = "Alpha";
            fakeShoppingListItemOne.Category = "Bravo";
            fakeShoppingListItemOne.ShoppingListId = 5;

            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemTwo.Name = "Charlie";
            fakeShoppingListItemTwo.Category = "Delta";
            fakeShoppingListItemTwo.ShoppingListId = 6;

            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemThree.Name = "Echo";
            fakeShoppingListItemThree.Category = "Foxtrot";
            fakeShoppingListItemThree.ShoppingListId = 7;

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { Filters = filters });

                //Assert
                ShoppingListItemRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("Name@=Hart", 1)]
        [InlineData("Category@=Fav", 1)]
        [InlineData("Name@=*hart", 2)]
        [InlineData("Category@=*fav", 2)]
        public void GetShoppingListItems_FilterListWithContains(string filters, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemOne.Name = "Alpha";
            fakeShoppingListItemOne.Category = "Bravo";

            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemTwo.Name = "Hartsfield";
            fakeShoppingListItemTwo.Category = "Favaro";

            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemThree.Name = "Bravehart";
            fakeShoppingListItemThree.Category = "Jonfav";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { Filters = filters });

                //Assert
                ShoppingListItemRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("hart", 1)]
        [InlineData("fav", 1)]
        [InlineData("Fav", 0)]
        public void GetShoppingListItems_SearchQueryReturnsExpectedRecordCount(string queryString, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CarbonKitchenDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemOne.Name = "Alpha";
            fakeShoppingListItemOne.Category = "Bravo";

            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemTwo.Name = "Hartsfield";
            fakeShoppingListItemTwo.Category = "White";

            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();
            fakeShoppingListItemThree.Name = "Bravehart";
            fakeShoppingListItemThree.Category = "Jonfav";

            //Act
            using (var context = new CarbonKitchenDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);
                context.SaveChanges();

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                var ShoppingListItemRepo = service.GetShoppingListItems(new ShoppingListItemParametersDto { QueryString = queryString });

                //Assert
                ShoppingListItemRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }
    }
}

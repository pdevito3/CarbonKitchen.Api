namespace CarbonKitchen.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.ShoppingListItem;
    using CarbonKitchen.Api.Tests.Fakes.ShoppingListItem;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class DeleteShoppingListItemRepositoryTests
    {
        [Fact]
        public void DeleteShoppingListItem_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<ShoppingListItemDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemThree = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new ShoppingListItemDbContext(dbOptions))
            {
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo, fakeShoppingListItemThree);

                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteShoppingListItem(fakeShoppingListItemTwo);

                context.SaveChanges();

                //Assert
                var ShoppingListItemList = context.ShoppingListItems.ToList();

                ShoppingListItemList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                ShoppingListItemList.Should().ContainEquivalentOf(fakeShoppingListItemOne);
                ShoppingListItemList.Should().ContainEquivalentOf(fakeShoppingListItemThree);
                Assert.DoesNotContain(ShoppingListItemList, i => i == fakeShoppingListItemTwo);

                context.Database.EnsureDeleted();
            }
        }
    }
}

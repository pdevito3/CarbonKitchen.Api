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
    using System.Linq;
    using Xunit;

    public class CreateShoppingListItemRepositoryTests
    {
        [Fact]
        public void AddShoppingListItem_NewRecordAddedWithProperValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<ShoppingListItemDbContext>()
                .UseInMemoryDatabase(databaseName: $"ShoppingListItemDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeShoppingListItem = new FakeShoppingListItem { }.Generate();

            //Act
            using (var context = new ShoppingListItemDbContext(dbOptions))
            {
                var service = new ShoppingListItemRepository(context, new SieveProcessor(sieveOptions));

                service.AddShoppingListItem(fakeShoppingListItem);

                context.SaveChanges();
            }

            //Assert
            using (var context = new ShoppingListItemDbContext(dbOptions))
            {
                context.ShoppingListItems.Count().Should().Be(1);

                var ShoppingListItemById = context.ShoppingListItems.FirstOrDefault(i => i.ShoppingListItemId == fakeShoppingListItem.ShoppingListItemId);

                ShoppingListItemById.Should().BeEquivalentTo(fakeShoppingListItem);
                ShoppingListItemById.ShoppingListItemId.Should().Be(fakeShoppingListItem.ShoppingListItemId);
                ShoppingListItemById.Name.Should().Be(fakeShoppingListItem.Name);
                ShoppingListItemById.Category.Should().Be(fakeShoppingListItem.Category);
                ShoppingListItemById.Unit.Should().Be(fakeShoppingListItem.Unit);
            }
        }
    }
}

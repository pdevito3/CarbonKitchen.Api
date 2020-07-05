namespace CarbonKitchen.Api.Tests.Fakes.ShoppingListItem
{
    using AutoBogus;
    using CarbonKitchen.Api.Models.ShoppingListItem;

    // or replace 'AutoFaker' with 'Faker' if you don't want all fields to be auto faked
    public class FakeShoppingListItemDto : AutoFaker<ShoppingListItemDto>
    {
        public FakeShoppingListItemDto()
        {
            // leaving the first 49 for potential special use cases in startup builds that need explicit values
            RuleFor(i => i.ShoppingListItemId, i => i.Random.Number(50, 100000));
        }
    }
}

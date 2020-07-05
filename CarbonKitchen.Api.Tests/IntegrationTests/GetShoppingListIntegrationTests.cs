namespace CarbonKitchen.Api.Tests.IntegrationTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using CarbonKitchen.Api.Tests.Fakes.ShoppingListItem;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class GetShoppingListItemIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public GetShoppingListItemIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private readonly CustomWebApplicationFactory<Startup> _factory;
        [Fact]
        public async Task GetShoppingListItems_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeShoppingListItemOne = new FakeShoppingListItem { }.Generate();
            var fakeShoppingListItemTwo = new FakeShoppingListItem { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShoppingListItemDbContext>();
                context.Database.EnsureCreated();

                context.ShoppingListItems.RemoveRange(context.ShoppingListItems);
                context.ShoppingListItems.AddRange(fakeShoppingListItemOne, fakeShoppingListItemTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync($"api/v1/ShoppingListItems")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<ShoppingListItemDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeShoppingListItemOne);
            response.Should().ContainEquivalentOf(fakeShoppingListItemTwo);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

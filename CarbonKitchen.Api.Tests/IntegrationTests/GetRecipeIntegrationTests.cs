namespace CarbonKitchen.Api.Tests.IntegrationTests
{
    using FluentAssertions;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Models.Recipe;
    using CarbonKitchen.Api.Tests.Fakes.Recipe;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class GetRecipeIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public GetRecipeIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private readonly CustomWebApplicationFactory<Startup> _factory;
        [Fact]
        public async Task GetRecipes_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var fakeRecipeTwo = new FakeRecipe { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
                context.Database.EnsureCreated();

                context.Recipes.RemoveRange(context.Recipes);
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync($"api/v1/recipes")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<RecipeDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeRecipeOne);
            response.Should().ContainEquivalentOf(fakeRecipeTwo);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

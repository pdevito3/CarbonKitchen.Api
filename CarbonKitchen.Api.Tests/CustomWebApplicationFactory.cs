﻿using CarbonKitchen.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CarbonKitchen.Api.Tests
{

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
            .ConfigureServices(services =>
            {
                // Remove the app's ShoppingListItemDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ShoppingListItemDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ShoppingListItemDbContext using an in-memory database for testing.
                services.AddDbContext<ShoppingListItemDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDb");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ShoppingListItemDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ShoppingListItemDbContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        db.RemoveRange(db.ShoppingListItems);
                        // Seed the database with test data.
                        //Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
        }

        public HttpClient GetAnonymousClient()
        {
            return CreateClient();
        }

        //public async Task<HttpClient> GetAuthenticatedClientAsync()
        //{
        //    return await GetAuthenticatedClientAsync("jason@northwind", "Northwind1!");
        //}

        //public async Task<HttpClient> GetAuthenticatedClientAsync(string userName, string password)
        //{
        //    var client = CreateClient();

        //    var token = await GetAccessTokenAsync(client, userName, password);

        //    client.SetBearerToken(token);

        //    return client;
        //}

        //    private async Task<string> GetAccessTokenAsync(HttpClient client, string userName, string password)
        //    {
        //        var disco = await client.GetDiscoveryDocumentAsync();

        //        if (disco.IsError)
        //        {
        //            throw new Exception(disco.Error);
        //        }

        //        var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        //        {
        //            Address = disco.TokenEndpoint,
        //            ClientId = "Northwind.IntegrationTests",
        //            ClientSecret = "secret",

        //            Scope = "Northwind.WebUIAPI openid profile",
        //            UserName = userName,
        //            Password = password
        //        });

        //        if (response.IsError)
        //        {
        //            throw new Exception(response.Error);
        //        }

        //        return response.AccessToken;
        //    }
        //}
    }
}

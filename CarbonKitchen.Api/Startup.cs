namespace CarbonKitchen.Api
{
    using AutoBogus;
    using Autofac;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using CarbonKitchen.Api.Data;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.ShoppingListItem;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sieve.Services;
    using System;

    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "MyCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<SieveProcessor>();
            
            services.AddScoped<IShoppingListItemRepository, ShoppingListItemRepository>();

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddDbContext<ShoppingListItemDbContext>(opt => 
                opt.UseInMemoryDatabase("ShoppingListItemDb"));

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
        public void ConfigureContainer(ContainerBuilder builder)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var context = app.ApplicationServices.GetService<ShoppingListItemDbContext>())
            {
                context.Database.EnsureCreated();

                // auto generate some fake data. added rules to accomodate placeholder validation rules
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Unit, fake => fake.Date.Past())
                    .RuleFor(fake => fake.ShoppingListId, fake => fake.Random.Number()));
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Unit, fake => fake.Date.Past())
                    .RuleFor(fake => fake.ShoppingListId, fake => fake.Random.Number()));
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Unit, fake => fake.Date.Past())
                    .RuleFor(fake => fake.ShoppingListId, fake => fake.Random.Number()));

                context.SaveChanges();
            }
        }
    }
}

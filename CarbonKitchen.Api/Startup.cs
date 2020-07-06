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
    using CarbonKitchen.Api.Services.Recipe;
    using CarbonKitchen.Api.Services.Ingredient;

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

            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IShoppingListItemRepository, ShoppingListItemRepository>();

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddDbContext<CarbonKitchenDbContext>(opt => 
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

            using (var context = app.ApplicationServices.GetService<CarbonKitchenDbContext>())
            {
                context.Database.EnsureCreated();
                
                context.Recipes.Add(new Recipe
                {
                    RecipeId = 1
                    , Title = "Instant Pot® Chicken and Wild Rice Soup"
                    , Directions = "Lorem ipsum dolor sit, amet consectetur adipisicing elit. Optio, molestias porro. Modi error iure facilis necessitatibus dolorum nam, \n \nitaque vero harum tenetur ullam officiis quo dolor porro voluptatibus inventore laboriosam, numquam at nobis ratione cupiditate? Ipsam, illo corporis. Quae harum quasi nostrum tempore\n \nmolestias error inventore assumenda animi, adipisci nam, tempora temporibus facere deleniti voluptatum non natus fugit ipsa porro quidem eaque minus. Facilis deleniti deserunt ullam! Aut quae debitis ea iusto minus mollitia modi nisi, aperiam quam iste qui, soluta a illum. Laudantium iure id voluptas itaque minima quisquam corporis quia vel fuga, iste nostrum omnis perspiciatis facere et dolore dolorem est deserunt sit, consectetur unde dolores expedita quas vitae. Hic nobis obcaecati animi illo, sint quo. Inventore natus quae similique ullam vero cumque nostrum culpa vitae blanditiis reiciendis, soluta incidunt, nam itaque facere autem labore iure numquam veritatis fuga perferendis. Excepturi laudantium magnam sed, tempore libero aliquam tenetur ea distinctio quam voluptate, repudiandae quo hic animi neque ducimus eaque blanditiis! Recusandae nam sed saepe odit sunt! Quisquam rerum ut beatae ratione quo odit consequatur dignissimos repudiandae, doloremque distinctio, saepe aliquid adipisci! Aliquid rerum repudiandae, esse animi quaerat ex quisquam nisi odit voluptate nobis, hic velit debitis est nam? Odit, nemo labore ducimus quibusdam qui soluta quis explicabo, commodi, cum aperiam impedit eveniet voluptatibus atque neque tempora cupiditate accusantium vel consectetur mollitia. Temporibus placeat quaerat, consectetur laborum est quia? A, corporis cum voluptates dignissimos, aliquam sapiente laboriosam aut possimus excepturi tempora temporibus nemo harum id recusandae. Libero adipisci quod cumque illo ratione odio. Commodi nulla accusamus quidem repudiandae delectus ipsum dicta ut repellat quibusdam amet nihil magnam vero dignissimos rem, eaque, consequatur placeat voluptatum ullam a quo, recusandae architecto nesciunt numquam ad. Aliquid, incidunt. Rem enim sint in quibusdam, magni est necessitatibus, impedit inventore molestias repellat consectetur aspernatur error dicta dolor perferendis debitis? Voluptatibus sit optio perferendis autem similique ratione totam saepe iusto debitis, et numquam ab quibusdam dicta voluptatem asperiores reprehenderit sapiente animi eligendi. Quibusdam vitae quam quasi nemo consequuntur cumque. Sapiente illum sunt corporis est recusandae adipisci, quisquam ex nisi voluptatum doloremque illo, praesentium et molestias vel quibusdam. Enim, cupiditate aperiam nemo quidem adipisci voluptate blanditiis officiis a cum sapiente distinctio, voluptatum ullam cumque commodi aliquam! Voluptatem veniam saepe, tenetur, ex cum eos at voluptates vitae, voluptatum repellat quia. Doloribus, cumque! Sit nihil odit libero, earum distinctio repellendus laudantium voluptas ipsum soluta! Doloremque aut quo eaque at nam excepturi repellendus velit eligendi animi eveniet necessitatibus tempora voluptas, recusandae ipsum veritatis accusamus minus. Ullam at quidem beatae rerum tempora. Odio possimus eos provident repudiandae molestiae eius quidem aspernatur alias recusandae nesciunt dolorum qui facere, distinctio, delectus, aliquam temporibus odit mollitia. Sequi incidunt aliquid aut libero ipsam aliquam. Fuga ex suscipit sit, consectetur esse voluptas, dicta repudiandae quos ratione perspiciatis nisi magnam! Quo quos voluptatem mollitia similique recusandae! Similique cupiditate vero distinctio hic officia! Reprehenderit aliquam officia voluptatibus vel doloremque pariatur numquam vero expedita! Dolore libero, saepe perspiciatis inventore provident maiores sunt odit. Repellendus eos dolorem ea alias harum."
                    , RecipeSourceLink = "https://www.bettycrocker.com/recipes/instant-pot-chicken-and-wild-rice-soup/8f631956-26b9-4353-8605-9f52cdde99db"
                    , Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Repudiandae, explicabo. Nostrum rerum delectus itaque. Distinctio at atque laborum hic? Dicta sequi quisquam voluptatibus labore quibusdam, iste accusamus ex aperiam voluptate!"
                    , ImageLink = "https://images-gmi-pmc.edge-generalmills.com/60c3ebda-50a7-415e-8c66-14f6c9b93034.jpg"
                  });
                context.Recipes.Add(new Recipe
                {
                    RecipeId =  2
                    , Title =  "The Only Red Velvet Cake Recipe You’ll Ever Need"
                    , Directions = "Lorem ipsum dolor sit, amet consectetur adipisicing elit. Optio, molestias porro. Modi error iure facilis necessitatibus dolorum nam, \n \nitaque vero harum tenetur ullam officiis quo dolor porro voluptatibus inventore laboriosam, numquam at nobis ratione cupiditate? Ipsam, illo corporis. Quae harum quasi nostrum tempore\n \nmolestias error inventore assumenda animi, adipisci nam, tempora temporibus facere deleniti voluptatum non natus fugit ipsa porro quidem eaque minus. Facilis deleniti deserunt ullam! Aut quae debitis ea iusto minus mollitia modi nisi, aperiam quam iste qui, soluta a illum. Laudantium iure id voluptas itaque minima quisquam corporis quia vel fuga, iste nostrum omnis perspiciatis facere et dolore dolorem est deserunt sit, consectetur unde dolores expedita quas vitae. Hic nobis obcaecati animi illo, sint quo. Inventore natus quae similique ullam vero cumque nostrum culpa vitae blanditiis reiciendis, soluta incidunt, nam itaque facere autem labore iure numquam veritatis fuga perferendis. Excepturi laudantium magnam sed, tempore libero aliquam tenetur ea distinctio quam voluptate, repudiandae quo hic animi neque ducimus eaque blanditiis! Recusandae nam sed saepe odit sunt! Quisquam rerum ut beatae ratione quo odit consequatur dignissimos repudiandae, doloremque distinctio, saepe aliquid adipisci! Aliquid rerum repudiandae, esse animi quaerat ex quisquam nisi odit voluptate nobis, hic velit debitis est nam? Odit, nemo labore ducimus quibusdam qui soluta quis explicabo, commodi, cum aperiam impedit eveniet voluptatibus atque neque tempora cupiditate accusantium vel consectetur mollitia. Temporibus placeat quaerat, consectetur laborum est quia? A, corporis cum voluptates dignissimos, aliquam sapiente laboriosam aut possimus excepturi tempora temporibus nemo harum id recusandae. Libero adipisci quod cumque illo ratione odio. Commodi nulla accusamus quidem repudiandae delectus ipsum dicta ut repellat quibusdam amet nihil magnam vero dignissimos rem, eaque, consequatur placeat voluptatum ullam a quo, recusandae architecto nesciunt numquam ad. Aliquid, incidunt. Rem enim sint in quibusdam, magni est necessitatibus, impedit inventore molestias repellat consectetur aspernatur error dicta dolor perferendis debitis? Voluptatibus sit optio perferendis autem similique ratione totam saepe iusto debitis, et numquam ab quibusdam dicta voluptatem asperiores reprehenderit sapiente animi eligendi. Quibusdam vitae quam quasi nemo consequuntur cumque. Sapiente illum sunt corporis est recusandae adipisci, quisquam ex nisi voluptatum doloremque illo, praesentium et molestias vel quibusdam. Enim, cupiditate aperiam nemo quidem adipisci voluptate blanditiis officiis a cum sapiente distinctio, voluptatum ullam cumque commodi aliquam! Voluptatem veniam saepe, tenetur, ex cum eos at voluptates vitae, voluptatum repellat quia. Doloribus, cumque! Sit nihil odit libero, earum distinctio repellendus laudantium voluptas ipsum soluta! Doloremque aut quo eaque at nam excepturi repellendus velit eligendi animi eveniet necessitatibus tempora voluptas, recusandae ipsum veritatis accusamus minus. Ullam at quidem beatae rerum tempora. Odio possimus eos provident repudiandae molestiae eius quidem aspernatur alias recusandae nesciunt dolorum qui facere, distinctio, delectus, aliquam temporibus odit mollitia. Sequi incidunt aliquid aut libero ipsam aliquam. Fuga ex suscipit sit, consectetur esse voluptas, dicta repudiandae quos ratione perspiciatis nisi magnam! Quo quos voluptatem mollitia similique recusandae! Similique cupiditate vero distinctio hic officia! Reprehenderit aliquam officia voluptatibus vel doloremque pariatur numquam vero expedita! Dolore libero, saepe perspiciatis inventore provident maiores sunt odit. Repellendus eos dolorem ea alias harum."
                    , RecipeSourceLink = "https://www.chefsteps.com/activities/the-only-red-velvet-cake-recipe-you-ll-ever-need"
                    , Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Repudiandae, explicabo. Nostrum rerum delectus itaque. Distinctio at atque laborum hic? Dicta sequi quisquam voluptatibus labore quibusdam, iste accusamus ex aperiam voluptate!"
                    , ImageLink = "https://cdn.copymethat.com/media/the_only_red_velvet_cake_recipe_youll_ev_20200214190801982058o69u7k.jpg"
                });

                context.Ingredients.Add(new Ingredient {
                    IngredientId = 1
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "package"
                    , Name = "(20 oz) boneless skinless chicken thighs"
                  });
                context.Ingredients.Add(new Ingredient { 
                    IngredientId = 2
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "teaspoon"
                    , Name = "salt"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 3
                    , RecipeId = 1
                    , Amount = .5
                    , Unit = "teaspoon"
                    , Name = "pepper"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 4
                    , RecipeId = 1
                    , Amount = 2
                    , Unit = "tablespoons"
                    , Name = "butter"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 5
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "package"
                    , Name = "(20 oz) boneless skinless chicken thighs"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 6
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "teaspoon"
                    , Name = "salt"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 7
                    , RecipeId = 1
                    , Amount = .5
                    , Unit = "teaspoon"
                    , Name = "pepper"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 8
                    , RecipeId = 1
                    , Amount = 2
                    , Unit = "tablespoons"
                    , Name = "butter"
                  });

                context.Ingredients.Add(new Ingredient {
                    IngredientId = 9
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "package"
                    , Name = "(20 oz) boneless skinless chicken thighs"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 10
                    , RecipeId = 1
                    , Amount = 1
                    , Unit = "teaspoon"
                    , Name = "salt"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 11
                    , RecipeId = 1
                    , Amount = .5
                    , Unit = "teaspoon"
                    , Name = "pepper"
                  });
                context.Ingredients.Add(new Ingredient {
                    IngredientId = 12
                    , RecipeId = 1
                    , Amount = 2
                    , Unit = "tablespoons"
                    , Name = "butter"
                  });

                // auto generate some fake data. added rules to accomodate placeholder validation rules
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Acquired, false)
                    .RuleFor(fake => fake.Hidden, false)
                    .RuleFor(fake => fake.Amount, fake => fake.Random.Number()));
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Acquired, false)
                    .RuleFor(fake => fake.Hidden, false)
                    .RuleFor(fake => fake.Amount, fake => fake.Random.Number()));
                context.ShoppingListItems.Add(new AutoFaker<ShoppingListItem>()
                    .RuleFor(fake => fake.Acquired, false)
                    .RuleFor(fake => fake.Hidden, false)
                    .RuleFor(fake => fake.Amount, fake => fake.Random.Number()));

                context.SaveChanges();
            }
        }
    }
}

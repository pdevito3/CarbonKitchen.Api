namespace CarbonKitchen.Api.Configuration
{
    using AutoMapper;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Recipe;

    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            //createmap<to this, from this>
            CreateMap<Recipe, RecipeDto>()
                .ReverseMap();
            CreateMap<RecipeForCreationDto, Recipe>();
            CreateMap<RecipeForUpdateDto, Recipe>()
                .ReverseMap();
        }
    }
}
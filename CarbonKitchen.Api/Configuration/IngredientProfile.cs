namespace CarbonKitchen.Api.Configuration
{
    using AutoMapper;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Ingredient;

    public class IngredientProfile : Profile
    {
        public IngredientProfile()
        {
            //createmap<to this, from this>
            CreateMap<Ingredient, IngredientDto>()
                .ReverseMap();
            CreateMap<IngredientForCreationDto, Ingredient>();
            CreateMap<IngredientForUpdateDto, Ingredient>()
                .ReverseMap();
        }
    }
}
namespace CarbonKitchen.Api.Configuration
{
    using AutoMapper;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.ShoppingListItem;

    public class ShoppingListItemProfile : Profile
    {
        public ShoppingListItemProfile()
        {
            //createmap<to this, from this>
            CreateMap<ShoppingListItem, ShoppingListItemDto>()
                .ReverseMap();
            CreateMap<ShoppingListItemForCreationDto, ShoppingListItem>();
            CreateMap<ShoppingListItemForUpdateDto, ShoppingListItem>()
                .ReverseMap();
        }
    }
}
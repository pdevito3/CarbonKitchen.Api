namespace CarbonKitchen.Api.Validators.ShoppingListItem
{
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ShoppingListItemForCreationDtoValidator : ShoppingListItemForManipulationDtoValidator<ShoppingListItemForCreationDto>
    {
        public ShoppingListItemForCreationDtoValidator()
        {
        }
    }
}

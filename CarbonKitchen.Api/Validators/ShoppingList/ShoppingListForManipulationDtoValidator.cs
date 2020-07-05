namespace CarbonKitchen.Api.Validators.ShoppingListItem
{
    using FluentValidation;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using System;

    public class ShoppingListItemForManipulationDtoValidator<T> : AbstractValidator<T> where T : ShoppingListItemForManipulationDto
    {
        public ShoppingListItemForManipulationDtoValidator()
        {
        }
    }
}

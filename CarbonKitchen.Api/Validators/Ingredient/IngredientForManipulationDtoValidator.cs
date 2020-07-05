namespace CarbonKitchen.Api.Validators.Ingredient
{
    using FluentValidation;
    using CarbonKitchen.Api.Models.Ingredient;
    using System;

    public class IngredientForManipulationDtoValidator<T> : AbstractValidator<T> where T : IngredientForManipulationDto
    {
        public IngredientForManipulationDtoValidator()
        {
            RuleFor(i => i.Name)
                .NotEmpty();
            RuleFor(i => i.RecipeId)
                .GreaterThanOrEqualTo(0);
        }
    }
}

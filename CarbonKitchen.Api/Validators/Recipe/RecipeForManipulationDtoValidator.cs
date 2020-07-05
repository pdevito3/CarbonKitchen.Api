namespace CarbonKitchen.Api.Validators.Recipe
{
    using FluentValidation;
    using CarbonKitchen.Api.Models.Recipe;
    using System;

    public class RecipeForManipulationDtoValidator<T> : AbstractValidator<T> where T : RecipeForManipulationDto
    {
        public RecipeForManipulationDtoValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty();
        }
    }
}

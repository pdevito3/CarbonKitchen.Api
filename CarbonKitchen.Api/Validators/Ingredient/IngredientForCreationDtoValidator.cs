namespace CarbonKitchen.Api.Validators.Ingredient
{
    using CarbonKitchen.Api.Models.Ingredient;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class IngredientForCreationDtoValidator : IngredientForManipulationDtoValidator<IngredientForCreationDto>
    {
        public IngredientForCreationDtoValidator()
        {
        }
    }
}

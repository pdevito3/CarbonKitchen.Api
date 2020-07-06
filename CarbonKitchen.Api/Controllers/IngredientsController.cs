namespace CarbonKitchen.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.Ingredient;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.Ingredient;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/ingredients")]
    public class IngredientsController : Controller
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(IIngredientRepository ingredientRepository
            , IMapper mapper)
        {
            _ingredientRepository = ingredientRepository ??
                throw new ArgumentNullException(nameof(ingredientRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetIngredients")]
        public ActionResult<IEnumerable<IngredientDto>> GetIngredients([FromQuery] IngredientParametersDto ingredientParametersDto)
        {
            var ingredientsFromRepo = _ingredientRepository.GetIngredients(ingredientParametersDto);
            
            var previousPageLink = ingredientsFromRepo.HasPrevious
                    ? CreateIngredientsResourceUri(ingredientParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = ingredientsFromRepo.HasNext
                ? CreateIngredientsResourceUri(ingredientParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = ingredientsFromRepo.TotalCount,
                pageSize = ingredientsFromRepo.PageSize,
                pageNumber = ingredientsFromRepo.PageNumber,
                totalPages = ingredientsFromRepo.TotalPages,
                hasPrevious = ingredientsFromRepo.HasPrevious,
                hasNext = ingredientsFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var ingredientsDto = _mapper.Map<IEnumerable<IngredientDto>>(ingredientsFromRepo);
            return Ok(ingredientsDto);
        }


        [HttpGet("{ingredientId}", Name = "GetIngredient")]
        public ActionResult<IngredientDto> GetIngredient(int ingredientId)
        {
            var ingredientFromRepo = _ingredientRepository.GetIngredient(ingredientId);

            if (ingredientFromRepo == null)
            {
                return NotFound();
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredientFromRepo);

            return Ok(ingredientDto);
        }

        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient(IngredientForCreationDto ingredientForCreation)
        {
            var ingredient = _mapper.Map<Ingredient>(ingredientForCreation);
            _ingredientRepository.AddIngredient(ingredient);
            _ingredientRepository.Save();

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);
            return CreatedAtRoute("GetIngredient",
                new { ingredientDto.IngredientId },
                ingredientDto);
        }

        [HttpPost("list/{recipeId}")]
        public ActionResult<IngredientDto> AddIngredientList(int recipeId, [FromBody] List<IngredientForCreationDto> ingredientsForCreation)
        {
            // delete the previous list
            /*var parameters = new IngredientParametersDto { Filters = $"RecipeId = {recipeId}", PageSize = 10 };
            var ingredientsFromRepo = _ingredientRepository.GetIngredients(parameters);*/

            _ingredientRepository.DeleteIngredients(recipeId);

            // then add the new one
            var ingredients = _mapper.Map<List<Ingredient>>(ingredientsForCreation);
            _ingredientRepository.AddIngredients(ingredients);

            // save and return new one
            _ingredientRepository.Save();

            var ingredientDto = _mapper.Map<List<IngredientDto>>(ingredients);
            return CreatedAtRoute("GetIngredients",
                1,
                //new { ingredientDto.RecipeId },
                ingredientDto);
        }

        [HttpDelete("{ingredientId}")]
        public ActionResult DeleteIngredient(int ingredientId)
        {
            var ingredientFromRepo = _ingredientRepository.GetIngredient(ingredientId);

            if (ingredientFromRepo == null)
            {
                return NotFound();
            }

            _ingredientRepository.DeleteIngredient(ingredientFromRepo);
            _ingredientRepository.Save();

            return NoContent();
        }

        [HttpPut("{ingredientId}")]
        public IActionResult UpdateIngredient(int ingredientId, IngredientForUpdateDto ingredient)
        {
            var ingredientFromRepo = _ingredientRepository.GetIngredient(ingredientId);

            if (ingredientFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredient, ingredientFromRepo);
            _ingredientRepository.UpdateIngredient(ingredientFromRepo);

            _ingredientRepository.Save();

            return NoContent();
        }

        [HttpPatch("{ingredientId}")]
        public IActionResult PartiallyUpdateIngredient(int ingredientId, JsonPatchDocument<IngredientForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetIngredient(ingredientId);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            var ingredientToPatch = _mapper.Map<IngredientForUpdateDto>(existingIngredient); // map the ingredient we got from the database to an updatable ingredient model
            patchDoc.ApplyTo(ingredientToPatch, ModelState); // apply patchdoc updates to the updatable ingredient

            if (!TryValidateModel(ingredientToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(ingredientToPatch, existingIngredient); // apply updates from the updatable ingredient to the db entity so we can apply the updates to the database
            _ingredientRepository.UpdateIngredient(existingIngredient); // apply business updates to data if needed

            _ingredientRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateIngredientsResourceUri(
            IngredientParametersDto ingredientParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetIngredients",
                        new
                        {
                            filters = ingredientParametersDto.Filters,
                            orderBy = ingredientParametersDto.SortOrder,
                            pageNumber = ingredientParametersDto.PageNumber - 1,
                            pageSize = ingredientParametersDto.PageSize,
                            searchQuery = ingredientParametersDto.QueryString
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetIngredients",
                        new
                        {
                            filters = ingredientParametersDto.Filters,
                            orderBy = ingredientParametersDto.SortOrder,
                            pageNumber = ingredientParametersDto.PageNumber + 1,
                            pageSize = ingredientParametersDto.PageSize,
                            searchQuery = ingredientParametersDto.QueryString
                        });

                default:
                    return Url.Link("GetIngredients",
                        new
                        {
                            filters = ingredientParametersDto.Filters,
                            orderBy = ingredientParametersDto.SortOrder,
                            pageNumber = ingredientParametersDto.PageNumber,
                            pageSize = ingredientParametersDto.PageSize,
                            searchQuery = ingredientParametersDto.QueryString
                        });
            }
        }
    }
}
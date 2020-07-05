namespace CarbonKitchen.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using CarbonKitchen.Api.Data.Entities;
    using CarbonKitchen.Api.Models.Pagination;
    using CarbonKitchen.Api.Models.ShoppingListItem;
    using CarbonKitchen.Api.Services;
    using CarbonKitchen.Api.Services.ShoppingListItem;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/ShoppingListItems")]
    public class ShoppingListItemsController : Controller
    {
        private readonly IShoppingListItemRepository _ShoppingListItemRepository;
        private readonly IMapper _mapper;

        public ShoppingListItemsController(IShoppingListItemRepository ShoppingListItemRepository
            , IMapper mapper)
        {
            _ShoppingListItemRepository = ShoppingListItemRepository ??
                throw new ArgumentNullException(nameof(ShoppingListItemRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetShoppingListItems")]
        public ActionResult<IEnumerable<ShoppingListItemDto>> GetCategories([FromQuery] ShoppingListItemParametersDto ShoppingListItemParametersDto)
        {
            var ShoppingListItemsFromRepo = _ShoppingListItemRepository.GetShoppingListItems(ShoppingListItemParametersDto);
            
            var previousPageLink = ShoppingListItemsFromRepo.HasPrevious
                    ? CreateShoppingListItemsResourceUri(ShoppingListItemParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = ShoppingListItemsFromRepo.HasNext
                ? CreateShoppingListItemsResourceUri(ShoppingListItemParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = ShoppingListItemsFromRepo.TotalCount,
                pageSize = ShoppingListItemsFromRepo.PageSize,
                pageNumber = ShoppingListItemsFromRepo.PageNumber,
                totalPages = ShoppingListItemsFromRepo.TotalPages,
                hasPrevious = ShoppingListItemsFromRepo.HasPrevious,
                hasNext = ShoppingListItemsFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var ShoppingListItemsDto = _mapper.Map<IEnumerable<ShoppingListItemDto>>(ShoppingListItemsFromRepo);
            return Ok(ShoppingListItemsDto);
        }


        [HttpGet("{ShoppingListItemId}", Name = "GetShoppingListItem")]
        public ActionResult<ShoppingListItemDto> GetShoppingListItem(int ShoppingListItemId)
        {
            var ShoppingListItemFromRepo = _ShoppingListItemRepository.GetShoppingListItem(ShoppingListItemId);

            if (ShoppingListItemFromRepo == null)
            {
                return NotFound();
            }

            var ShoppingListItemDto = _mapper.Map<ShoppingListItemDto>(ShoppingListItemFromRepo);

            return Ok(ShoppingListItemDto);
        }

        [HttpPost]
        public ActionResult<ShoppingListItemDto> AddShoppingListItem(ShoppingListItemForCreationDto ShoppingListItemForCreation)
        {
            var ShoppingListItem = _mapper.Map<ShoppingListItem>(ShoppingListItemForCreation);
            _ShoppingListItemRepository.AddShoppingListItem(ShoppingListItem);
            _ShoppingListItemRepository.Save();

            var ShoppingListItemDto = _mapper.Map<ShoppingListItemDto>(ShoppingListItem);
            return CreatedAtRoute("GetShoppingListItem",
                new { ShoppingListItemDto.ShoppingListItemId },
                ShoppingListItemDto);
        }

        [HttpDelete("{ShoppingListItemId}")]
        public ActionResult DeleteShoppingListItem(int ShoppingListItemId)
        {
            var ShoppingListItemFromRepo = _ShoppingListItemRepository.GetShoppingListItem(ShoppingListItemId);

            if (ShoppingListItemFromRepo == null)
            {
                return NotFound();
            }

            _ShoppingListItemRepository.DeleteShoppingListItem(ShoppingListItemFromRepo);
            _ShoppingListItemRepository.Save();

            return NoContent();
        }

        [HttpPut("{ShoppingListItemId}")]
        public IActionResult UpdateShoppingListItem(int ShoppingListItemId, ShoppingListItemForUpdateDto ShoppingListItem)
        {
            var ShoppingListItemFromRepo = _ShoppingListItemRepository.GetShoppingListItem(ShoppingListItemId);

            if (ShoppingListItemFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(ShoppingListItem, ShoppingListItemFromRepo);
            _ShoppingListItemRepository.UpdateShoppingListItem(ShoppingListItemFromRepo);

            _ShoppingListItemRepository.Save();

            return NoContent();
        }

        [HttpPatch("{ShoppingListItemId}")]
        public IActionResult PartiallyUpdateShoppingListItem(int ShoppingListItemId, JsonPatchDocument<ShoppingListItemForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingShoppingListItem = _ShoppingListItemRepository.GetShoppingListItem(ShoppingListItemId);

            if (existingShoppingListItem == null)
            {
                return NotFound();
            }

            var ShoppingListItemToPatch = _mapper.Map<ShoppingListItemForUpdateDto>(existingShoppingListItem); // map the ShoppingListItem we got from the database to an updatable ShoppingListItem model
            patchDoc.ApplyTo(ShoppingListItemToPatch, ModelState); // apply patchdoc updates to the updatable ShoppingListItem

            if (!TryValidateModel(ShoppingListItemToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(ShoppingListItemToPatch, existingShoppingListItem); // apply updates from the updatable ShoppingListItem to the db entity so we can apply the updates to the database
            _ShoppingListItemRepository.UpdateShoppingListItem(existingShoppingListItem); // apply business updates to data if needed

            _ShoppingListItemRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateShoppingListItemsResourceUri(
            ShoppingListItemParametersDto ShoppingListItemParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetShoppingListItems",
                        new
                        {
                            filters = ShoppingListItemParametersDto.Filters,
                            orderBy = ShoppingListItemParametersDto.SortOrder,
                            pageNumber = ShoppingListItemParametersDto.PageNumber - 1,
                            pageSize = ShoppingListItemParametersDto.PageSize,
                            searchQuery = ShoppingListItemParametersDto.QueryString
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetShoppingListItems",
                        new
                        {
                            filters = ShoppingListItemParametersDto.Filters,
                            orderBy = ShoppingListItemParametersDto.SortOrder,
                            pageNumber = ShoppingListItemParametersDto.PageNumber + 1,
                            pageSize = ShoppingListItemParametersDto.PageSize,
                            searchQuery = ShoppingListItemParametersDto.QueryString
                        });

                default:
                    return Url.Link("GetShoppingListItems",
                        new
                        {
                            filters = ShoppingListItemParametersDto.Filters,
                            orderBy = ShoppingListItemParametersDto.SortOrder,
                            pageNumber = ShoppingListItemParametersDto.PageNumber,
                            pageSize = ShoppingListItemParametersDto.PageSize,
                            searchQuery = ShoppingListItemParametersDto.QueryString
                        });
            }
        }
    }
}
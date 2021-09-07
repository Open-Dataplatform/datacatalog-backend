using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="includeEmpty">If set to true the returned category list will include categories without datasets</param>
        /// <returns>A list of all categories</returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllAsync(bool? includeEmpty = false)
        {
            var categories = await _categoryService.ListAsync(includeEmpty ?? true);

            if (categories == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Category>, IEnumerable<CategoryResponse>>(categories);
            
            return Ok(result);
        }

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="id">The id of the category to get</param>
        /// <returns>The category</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CategoryResponse>> Get(Guid id)
        {
            var category = await _categoryService.FindByIdAsync(id);

            if (category == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Category, CategoryResponse>(category);

            return Ok(result);
        }

        /// <summary>
        /// Create a new category
        /// </summary> 
        /// <param name="request">The category to create</param>
        /// <returns>The created category</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> PostAsync([FromBody] CategoryCreateRequest request)
        {
            var category = _mapper.Map<CategoryCreateRequest, Data.Domain.Category>(request);
            category.CreatedDate = DateTime.UtcNow;
            category.ModifiedDate = DateTime.UtcNow;
            
            var createdCategory = await _categoryService.SaveAsync(category);
            var result = _mapper.Map<Data.Domain.Category, CategoryResponse>(category);

            return Ok(result);
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="request">The category to update</param>
        /// <returns>The updated category Id</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<ActionResult<CategoryResponse>> PutAsync([FromBody] CategoryUpdateRequest request)
        {
            var category = _mapper.Map<CategoryUpdateRequest, Data.Domain.Category>(request);
            var updatedCategory = await _categoryService.UpdateAsync(category);
            var result = _mapper.Map<Data.Domain.Category, CategoryResponse>(updatedCategory);

            return Ok(result);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <remarks>Categories with references to dataset cannot be deleted!</remarks>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _categoryService.DeleteAsync(id);

            return Ok();
        }
    }
}
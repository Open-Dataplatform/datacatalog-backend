using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Data;
using DataCatalog.Api.Extensions;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward)]
    [ApiController]
    [Route("api/[controller]")]
    public class TransformationController : ControllerBase
    {
        private readonly ITransformationService _transformationService;
        private readonly IMapper _mapper;

        public TransformationController(ITransformationService transformationService, IMapper mapper)
        {
            _transformationService = transformationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all transformations
        /// </summary>
        /// <returns>A list of all transformations</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransformationResponse>>> GetAllAsync()
        {
            var transformations = await _transformationService.ListAsync();

            if (transformations == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Transformation>, IEnumerable<TransformationResponse>>(transformations);

            return Ok(result);
        }

        /// <summary>
        /// Get a transformation by id
        /// </summary>
        /// <param name="id">The id of the transformation to get</param>
        /// <returns>The category</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TransformationResponse>> Get(Guid id)
        {
            var transformation = await _transformationService.FindByIdAsync(id);

            if (transformation == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Transformation, TransformationResponse>(transformation);

            return Ok(result);
        }

        /// <summary>
        /// Get transformations where the sources are the given datasets
        /// </summary>
        /// <param name="datasets">The datasets to find transformations for</param>
        /// <returns>A list of transformations</returns>
        [HttpPost]
        [Route("getbydatasets")]
        public async Task<ActionResult<TransformationResponse[]>> GetByDatasetsAsync(GuidId[] datasets)
        {
            var transformations = await _transformationService.GetByDatasetIdsAsync(datasets.Select(d => d.Id).ToArray());

            if (transformations == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Transformation>, IEnumerable<TransformationResponse>>(transformations);

            return Ok(result);
        }

        /// <summary>
        /// Create a new transformation
        /// </summary> 
        /// <param name="request">The transformation to create</param>
        /// <returns>The created transformation</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<Guid>> PostAsync([FromBody] TransformationCreateRequest request)
        {
            var transformation = _mapper.Map<TransformationCreateRequest, Data.Domain.Transformation>(request);
            await _transformationService.SaveAsync(transformation);

            return Ok(transformation.Id);
        }

        /// <summary>
        /// Update transformation
        /// </summary>
        /// <param name="request">The transformation to create</param>
        /// <returns>The created transformation</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<ActionResult<Guid>> PutAsync([FromBody] TransformationUpdateRequest request)
        {
            var transformation = _mapper.Map<TransformationUpdateRequest, Data.Domain.Transformation>(request);
            await _transformationService.UpdateAsync(transformation);

            return Ok(transformation.Id);
        }

        /// <summary>
        /// Delete a transformation
        /// </summary>
        /// <param name="id">The id of the transformation to delete</param>
        /// <remarks>Deleting a transformation will affect the lineage of all sink and source datasets!</remarks>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _transformationService.DeleteAsync(id);

            return Ok();
        }
    }
}

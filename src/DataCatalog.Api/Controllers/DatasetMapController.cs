using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Repositories;
using DataCatalog.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatasetMapController : ControllerBase
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IMapper _mapper;

        public DatasetMapController(IDatasetRepository datasetRepository, IMapper mapper)
        {
            _datasetRepository = datasetRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all datasets
        /// </summary>
        /// <returns>A list of dataset summaries</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatasetMapResponse>>> GetDatasetMap()
        {
            var datasets = await _datasetRepository.ListAllNonDeletedDatasets();
            var result = _mapper.Map<IEnumerable<Dataset>, IEnumerable<DatasetMapResponse>>(datasets);

            return Ok(result);
        }
    }
}
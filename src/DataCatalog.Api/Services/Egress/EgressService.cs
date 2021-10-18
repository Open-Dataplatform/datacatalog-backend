using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataCatalog.Api.Data;
using DataCatalog.Common.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace DataCatalog.Api.Services.Egress
{
    public class EgressService : IEgressService
    {
        private const int Limit = 10;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EgressService> _logger;
        private readonly string _egressBaseUrl; 

        public EgressService(ILogger<EgressService> logger, HttpClient httpClient, IOptions<EgressOptions> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _egressBaseUrl = options.Value.BaseUrl;
            if (!options.Value.BaseUrl.EndsWith('/'))
            {
                _egressBaseUrl += '/';
            }
        }

        public async Task<Either<object, Exception>> FetchData(Guid datasetId, string fromDate, string toDate, string authorizationHeader)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_egressBaseUrl}{datasetId}/json?limit={Limit}&from_date={fromDate}&to_date={toDate}");
            request.Headers.Add("Authorization", authorizationHeader);
            _logger.LogInformation("Fetching {Limit} rows from the dataset with Id {DatasetId} within the time range of {FromDate} to {ToDate}", Limit, datasetId, fromDate, toDate);
            var response = await _httpClient.SendAsync(request);
            var stringJson =  await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var exceptionDetails = JsonSerializer.Deserialize<ExceptionDetails>(stringJson, options);
                using (LogContext.PushProperty("ExceptionDetails", exceptionDetails, true))
                {
                    _logger.LogInformation("Failed to fetch data from the Egress API. The API returned with status code {StatusCode}", response.StatusCode);
                }
                return response.StatusCode switch
                {
                    HttpStatusCode.Forbidden => new Either<object, Exception>(
                        new AuthorizationException(exceptionDetails?.Detail)),
                    HttpStatusCode.BadRequest => new Either<object, Exception>(
                        new ConfigurationException(exceptionDetails?.Detail)),
                    _ => new Either<object, Exception>(new Exception(exceptionDetails?.Detail))
                };
            }
            
            var result = JsonSerializer.Deserialize<object>(stringJson);
            return new Either<object, Exception>(result);
        }

        public class ExceptionDetails
        {
            public string Detail { get; set; }
        }
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Egress
{
    public class EgressService : IEgressService
    {
        private const int Limit = 10;
        private readonly HttpClient _httpClient = new();
        
        public async Task<Either<object, Exception>> FetchData(Guid datasetId, string fromDate, string toDate, string authorizationHeader)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://dp-test.westeurope.cloudapp.azure.com/osiris-egress/v1/{datasetId}/json?limit={Limit}&from_date={fromDate}&to_date={toDate}");
            request.Headers.Add("Authorization", authorizationHeader);
            var response = await _httpClient.SendAsync(request);
            var stringJson =  await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var exceptionDetails = JsonSerializer.Deserialize<ExceptionDetails>(stringJson, options);
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
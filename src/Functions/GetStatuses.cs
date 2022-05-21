using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.CircuitBreaker;
using CircuitBreakerService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CircuitBreakerService.Functions
{
    public class GetStatuses
    {
        private readonly ICircuitBreakerFactory circuitBreakerFactory;

        public GetStatuses(ICircuitBreakerFactory circuitBreakerFactory)
        {
            this.circuitBreakerFactory = circuitBreakerFactory;
        }

        [FunctionName("GetStatuses")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.GetStatuses)] HttpRequestMessage req)
        {
            var circuitBreakers = await this.circuitBreakerFactory.GetAllCircuitBreakersAsync();

            List<ResponseDto> response = new List<ResponseDto>();
            foreach (var circuitBreaker in  circuitBreakers)
            {
                var (circuitState, retryAfter) = await circuitBreaker.GetCircuitStateAsync();
                
                response.Add(new ResponseDto()
                {
                    Key = circuitBreaker.Key,
                    RetryAfterInSeconds = retryAfter.HasValue ? Math.Round(retryAfter.Value.TotalSeconds, 0) : (double?)null,
                    CircuitState = circuitState,
                });
            }

            return new OkObjectResult(response);
        }

        class ResponseDto
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("retryAfterInSeconds", NullValueHandling = NullValueHandling.Ignore)]
            public double? RetryAfterInSeconds { get; set; }
            
            [JsonProperty("circuitState")]
            [JsonConverter(typeof(StringEnumConverter))]
            public CircuitState CircuitState { get; set; }
        }
    }
}

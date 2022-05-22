using System;
using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.Core.Models;
using CircuitBreakerService.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CircuitBreakerService.Functions
{
    public class GetStatus
    {
        private readonly ICircuitBreakerFactory circuitBreakerFactory;

        public GetStatus(ICircuitBreakerFactory circuitBreakerFactory)
        {
            this.circuitBreakerFactory = circuitBreakerFactory;
        }

        [FunctionName("GetStatus")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.GetStatus)] HttpRequestMessage req,
            [FromRoute] string key)
        {
            var circuitBreaker = await this.circuitBreakerFactory.GetCircuitBreakerAsync(key);
            if (circuitBreaker == null)
            {
                return new NotFoundResult();
            }
            
            var (circuitState, retryAfter) = await circuitBreaker.GetCircuitStateAsync();

            var response = new ResponseDto()
            {
                Key = key,
                RetryAfterInSeconds = retryAfter.HasValue ? Math.Round(retryAfter.Value.TotalSeconds, 0) : (double?) null,
                CircuitState = circuitState,
            };

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

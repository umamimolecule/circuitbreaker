using System;
using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.Extensions;
using CircuitBreakerService.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace CircuitBreakerService.Functions
{
    public class CallFailure
    {
        private readonly ICircuitBreakerFactory circuitBreakerFactory;

        public CallFailure(ICircuitBreakerFactory circuitBreakerFactory)
        {
            this.circuitBreakerFactory = circuitBreakerFactory;
        }

        [FunctionName("CallFailure")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Routes.CallFailure)] HttpRequestMessage req,
            [FromRoute] string key)
        {
            var bodyValidation = await req.ValidateBodyAsync<RequestDto>(true);
            if (!bodyValidation.IsValid)
            {
                return bodyValidation.CreateErrorResponse();
            }
            
            var circuitBreaker = await this.circuitBreakerFactory.GetCircuitBreakerAsync(key);
            await circuitBreaker.RecordFailureAsync(bodyValidation.Body?.GetRetryAfterAsTimeSpan());
            return new OkResult();
        }

        class RequestDto
        {
            [JsonProperty("retryAfterInSeconds", NullValueHandling = NullValueHandling.Ignore)]
            public double? RetryAfterInSeconds { get; set; }

            public TimeSpan? GetRetryAfterAsTimeSpan()
            {
                return this.RetryAfterInSeconds == null
                    ? (TimeSpan?)null
                    : TimeSpan.FromSeconds(this.RetryAfterInSeconds.Value);
            }
        }
    }
}

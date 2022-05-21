using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CircuitBreakerService.Functions
{
    public class CallSuccess
    {
        private readonly ICircuitBreakerFactory circuitBreakerFactory;

        public CallSuccess(ICircuitBreakerFactory circuitBreakerFactory)
        {
            this.circuitBreakerFactory = circuitBreakerFactory;
        }

        [FunctionName("CallSuccess")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Routes.CallSuccess)]
            HttpRequestMessage req,
            [FromRoute] string key)
        {
            var circuitBreaker = await this.circuitBreakerFactory.GetCircuitBreakerAsync(key);
            await circuitBreaker.RecordSuccessAsync();
            return new OkResult();
        }
    }
}

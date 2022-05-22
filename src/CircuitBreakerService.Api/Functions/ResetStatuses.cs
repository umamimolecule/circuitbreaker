using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CircuitBreakerService.Functions
{
    public class ResetStatuses
    {
        private readonly ICircuitBreakerFactory circuitBreakerFactory;

        public ResetStatuses(ICircuitBreakerFactory circuitBreakerFactory)
        {
            this.circuitBreakerFactory = circuitBreakerFactory;
        }

        [FunctionName("ResetStatuses")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Routes.ResetStatuses)] HttpRequestMessage req)
        {
            await this.circuitBreakerFactory.ClearAllCircuitBreakersAsync();
            return new OkResult();
        }
    }
}

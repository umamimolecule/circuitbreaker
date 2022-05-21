using CircuitBreakerService.RateLimiting;
using CircuitBreakerService.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CircuitBreakerService.Startup))]

namespace CircuitBreakerService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IRateLimitedService, RateLimitedService>();
            builder.Services.AddSingleton<ICircuitBreakerFactory, InMemoryCircuitBreakerFactory>();
        }
    }
}

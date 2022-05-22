using CircuitBreakerService.Core.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CircuitBreakerService.Startup))]

namespace CircuitBreakerService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICircuitBreakerFactory, InMemoryCircuitBreakerFactory>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircuitBreakerService.RateLimiting
{
    public interface IRateLimitedService
    {
        string Name { get; }
        
        Task<IDictionary<string, object>> GetConfigurationAsync();
        
        Task<ConfigurationValidationResult> ConfigureAsync(IDictionary<string, object> configuration);
        
        Task RunAsync(string key, Func<Task> func);
    }
}
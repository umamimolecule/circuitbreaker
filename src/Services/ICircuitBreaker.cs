using System;
using System.Threading.Tasks;
using CircuitBreakerService.CircuitBreaker;

namespace CircuitBreakerService.Services
{
    public interface ICircuitBreaker
    {
        string Key { get; }
       
        Task<CircuitBreakerResult> IsExecutionPermittedAsync();

        Task<CircuitState> RecordSuccessAsync();
        
        Task<CircuitState> RecordFailureAsync(TimeSpan? retryAfter);
        
        Task<(CircuitState, TimeSpan?)> GetCircuitStateAsync();
    }
}
﻿using CircuitBreakerService.Core.Models;

namespace CircuitBreakerService.Core.Services
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
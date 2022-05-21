using System;

namespace CircuitBreakerService.CircuitBreaker
{
    public struct CircuitBreakerResult
    {
        public bool IsExecutionPermitted { get; set; }
        
        public TimeSpan? RetryAfter { get; set; }
    }
}
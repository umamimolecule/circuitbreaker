namespace CircuitBreakerService.Core.Models
{
    public struct CircuitBreakerResult
    {
        public bool IsExecutionPermitted { get; set; }
        
        public TimeSpan? RetryAfter { get; set; }
    }
}
namespace CircuitBreakerService.Core.Services
{
    public interface ICircuitBreakerFactory
    {
        Task<ICircuitBreaker> GetCircuitBreakerAsync(string key);
        
        Task<IEnumerable<ICircuitBreaker>> GetAllCircuitBreakersAsync();
        
        Task ClearAllCircuitBreakersAsync();
    }
}
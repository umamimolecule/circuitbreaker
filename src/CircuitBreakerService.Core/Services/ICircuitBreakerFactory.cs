using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircuitBreakerService.Core.Services
{
    public interface ICircuitBreakerFactory
    {
        Task<ICircuitBreaker> GetCircuitBreakerAsync(string key);
        
        Task<IEnumerable<ICircuitBreaker>> GetAllCircuitBreakersAsync();
        
        Task ClearAllCircuitBreakersAsync();
    }
}
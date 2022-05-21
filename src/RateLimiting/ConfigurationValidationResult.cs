using System.Collections.Generic;
using System.Linq;

namespace CircuitBreakerService.RateLimiting
{
    public class ConfigurationValidationResult
    {
        public bool IsValid => this.Errors == null || !this.Errors.Any();
        
        public IEnumerable<string> Errors { get; set; }
    }
}
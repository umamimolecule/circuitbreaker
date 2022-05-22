using System.Collections.Generic;
using System.Linq;

namespace CircuitBreakerService.Models
{
    public abstract class ValidateResult
    {
        public bool IsValid => this.Errors == null || this.Errors.Count() == 0;

        public IEnumerable<string> Errors { get; set; }
    }
}

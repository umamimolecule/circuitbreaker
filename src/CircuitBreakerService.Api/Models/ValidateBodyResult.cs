namespace CircuitBreakerService.Models
{
    public class ValidateBodyResult<T> : ValidateResult
    {
        public T Body { get; set; }
    }
}

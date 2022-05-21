namespace CircuitBreakerService.Models
{
    public class ValidateQueryResult<T> : ValidateResult
    {
        public T Query { get; set; }
    }
}

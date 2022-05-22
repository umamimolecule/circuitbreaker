using CircuitBreakerService.Dtos;
using CircuitBreakerService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreakerService.Extensions
{
    public static class ValidateResultExtensions
    {
        public static IActionResult CreateErrorResponse(this ValidateResult result)
        {
            var error = new ErrorResponse(result.Errors);
            return new BadRequestObjectResult(error);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CircuitBreakerService.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace CircuitBreakerService.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static async Task<ValidateBodyResult<T>> ValidateBodyAsync<T>(this HttpRequestMessage httpRequestMessage, bool allowNull = false)
        {
            var content = await httpRequestMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                if (allowNull)
                {
                    return new ValidateBodyResult<T>();
                }

                return new ValidateBodyResult<T>()
                {
                    Errors = new string[] { "Body required" },
                };
            }

            var body = JsonConvert.DeserializeObject<T>(content);

            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(body, new ValidationContext(body), validationResults, true))
            {
                return new ValidateBodyResult<T>()
                {
                    Body = body,
                };
            }

            return new ValidateBodyResult<T>()
            {
                Errors = validationResults.Select(x => $"{string.Join(", ", x.MemberNames)}: {x.ErrorMessage}"),
            };
        }

        public static ValidateQueryResult<T> ValidateQuery<T>(this HttpRequestMessage httpRequestMessage)
        {
            UriBuilder builder = new UriBuilder(httpRequestMessage.RequestUri);
            var queryParameters = QueryHelpers.ParseQuery(httpRequestMessage.RequestUri.Query)
                .ToDictionary(x => x.Key, x => x.Value.ToString());
            var query = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(queryParameters));

            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(query, new ValidationContext(query), validationResults, true))
            {
                return new ValidateQueryResult<T>()
                {
                    Query = query,
                };
            }

            return new ValidateQueryResult<T>()
            {
                Errors = validationResults.Select(x => $"{string.Join(", ", x.MemberNames)}: {x.ErrorMessage}"),
            };
        }
    }
}

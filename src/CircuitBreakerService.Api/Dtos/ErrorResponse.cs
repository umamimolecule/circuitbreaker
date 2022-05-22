using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CircuitBreakerService.Dtos
{
    public class ErrorResponse
    {
        public ErrorResponse(
            IEnumerable<string> errors,
            string correlationId = null)
        {
            this.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            this.Error = new ErrorResponseBody()
            {
                Messages = errors,
            };
        }

        public ErrorResponse(
            string error,
            string correlationId = null)
        {
            this.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            this.Error = new ErrorResponseBody()
            {
                Message = error,
            };
        }

        [JsonProperty("error")]
        public ErrorResponseBody Error { get; set; }

        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        public class ErrorResponseBody
        {
            [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
            public string Message { get; set; }

            [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
            public IEnumerable<string> Messages { get; set; }
        }
    }
}

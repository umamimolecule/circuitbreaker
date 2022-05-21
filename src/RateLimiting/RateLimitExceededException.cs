using System;
using System.Runtime.Serialization;

namespace CircuitBreakerService.RateLimiting
{
    [Serializable]
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string key, TimeSpan retryAfter)
            : base("Too many requests")
        {
            this.Key = key;
            this.RetryAfter = retryAfter;
        }

        public RateLimitExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Key = info.GetString(nameof(this.Key));
            this.RetryAfter = TimeSpan.FromSeconds(info.GetDouble(nameof(this.RetryAfter)));
        }

        public string Key { get; private set; }
        
        public TimeSpan RetryAfter { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.Key), this.Key);
            info.AddValue(nameof(this.RetryAfter), this.RetryAfter.TotalSeconds);
        }
    }
}
using System.Collections.Generic;

namespace CircuitBreakerService.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.TryAdd(key, value))
            {
                @this[key] = value;
            }
        }
    }
}
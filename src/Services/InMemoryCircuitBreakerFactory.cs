using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CircuitBreakerService.Extensions;
using Microsoft.Extensions.Logging;

namespace CircuitBreakerService.Services
{
    public class InMemoryCircuitBreakerFactory : ICircuitBreakerFactory
    {
        private readonly ILogger<ConsecutiveFailureCircuitBreaker> consecutiveFailureCircuitBreakerLogger; 
        private readonly Dictionary<string, ICircuitBreaker> cache = new Dictionary<string, ICircuitBreaker>();
        private readonly Dictionary<string, DateTime> cacheTimestamps = new Dictionary<string, DateTime>();
        private Timer cacheCleanupTimer;
        private TimeSpan cacheLifetime;

        public InMemoryCircuitBreakerFactory(
            ILogger<ConsecutiveFailureCircuitBreaker> consecutiveFailureCircuitBreakerLogger)
        {
            this.consecutiveFailureCircuitBreakerLogger = consecutiveFailureCircuitBreakerLogger;
            
            this.cacheLifetime = TimeSpan.FromMinutes(1);

            this.cacheCleanupTimer = new Timer();
            this.cacheCleanupTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            this.cacheCleanupTimer.Enabled = true;
            this.cacheCleanupTimer.AutoReset = true;
            this.cacheCleanupTimer.Start();
            this.cacheCleanupTimer.Elapsed += (sender, args) =>
            {
                lock (this.cache)
                {
                    var keys = this.cacheTimestamps
                        .Where(x => DateTime.UtcNow - x.Value > this.cacheLifetime)
                        .Select(x => x.Key);

                    foreach (var key in keys)
                    {
                        this.cache.Remove(key);
                        this.cacheTimestamps.Remove(key);
                    }
                }
            };
        }

        public Task<ICircuitBreaker> GetCircuitBreakerAsync(string key)
        {
            lock (this.cache)
            {
                if (!this.cache.TryGetValue(key, out var circuitBreaker))
                {
                    circuitBreaker = this.CreateCircuitBreaker(key);
                    this.cache.Add(key, circuitBreaker);
                }

                this.cacheTimestamps.AddOrUpdate(key, DateTime.UtcNow);
                
                return Task.FromResult(circuitBreaker);
            }
        }

        public Task<IEnumerable<ICircuitBreaker>> GetAllCircuitBreakersAsync()
        {
            lock (this.cache)
            {
                return Task.FromResult(this.cache.Values.AsEnumerable());
            }
        }

        public Task ClearAllCircuitBreakersAsync()
        {
            lock (this.cache)
            {
                this.cache.Clear();
                this.cacheTimestamps.Clear();
                return Task.CompletedTask;
            }
        }

        private ICircuitBreaker CreateCircuitBreaker(string key) => new ConsecutiveFailureCircuitBreaker(consecutiveFailureCircuitBreakerLogger, key);
    }
}
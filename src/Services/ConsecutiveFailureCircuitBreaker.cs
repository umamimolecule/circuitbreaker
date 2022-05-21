using System;
using System.Threading.Tasks;
using CircuitBreakerService.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace CircuitBreakerService.Services
{
    public class ConsecutiveFailureCircuitBreaker : ICircuitBreaker
    {
        private DateTime brokenUntil;

        private CircuitState circuitState;

        private int consecutiveFailureCount;

        private readonly int maxConsecutiveFailures;

        private readonly TimeSpan breakDuration;

        private readonly ILogger<ConsecutiveFailureCircuitBreaker> logger;

        private static readonly object Semaphore = new object();
        
        public ConsecutiveFailureCircuitBreaker(
            ILogger<ConsecutiveFailureCircuitBreaker> logger,
            string id,
            TimeSpan? breakDuration = null,
            int? maxConsecutiveFailures = null)
        {
            this.logger = logger;
            this.Key = id;
            this.breakDuration = breakDuration ?? TimeSpan.FromSeconds(30);
            this.maxConsecutiveFailures = maxConsecutiveFailures ?? 0;
        }
        
        public string Key { get; }

        public async Task<CircuitBreakerResult> IsExecutionPermittedAsync()
        {
            await Task.CompletedTask;
            
            switch (this.circuitState)
            {
                case CircuitState.Closed:
                    return new CircuitBreakerResult()
                    {
                        IsExecutionPermitted = true,
                    };

                case CircuitState.Open:
                case CircuitState.HalfOpen:

                    // When the breaker is Open or HalfOpen, we permit a single test execution after BreakDuration has passed.
                    // The test execution phase is known as HalfOpen state.
                    lock (Semaphore)
                    {
                        if (DateTime.UtcNow > this.brokenUntil)
                        {
                            this.circuitState = CircuitState.HalfOpen;
                            this.brokenUntil = DateTime.UtcNow + this.breakDuration;

                            this.logger.LogDebug("[Key={0}] Permitting a test execution in half-open state, set brokenUntil to {1:o}", this.Key, this.brokenUntil);

                            return new CircuitBreakerResult()
                            {
                                IsExecutionPermitted = true,
                            };
                        }

                        // Not time yet to test the circuit again.
                        return new CircuitBreakerResult()
                        {
                            IsExecutionPermitted = false,
                            RetryAfter = this.brokenUntil - DateTime.UtcNow,
                        };
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<CircuitState> RecordSuccessAsync()
        {
            await Task.CompletedTask;

            lock (Semaphore)
            {
                this.consecutiveFailureCount = 0;
                if (this.IsHalfOpen())
                {
                    this.brokenUntil = DateTime.MinValue;
                    this.circuitState = CircuitState.Closed;   
                    this.logger.LogDebug("[Key={0}] Circuit re-closed, set brokenUntil to {1:o}", this.Key, this.brokenUntil);
                }

                return this.circuitState;
            }
        }

        public async Task<CircuitState> RecordFailureAsync(TimeSpan? retryAfter)
        {
            await Task.CompletedTask;
            lock (Semaphore)
            {
                this.consecutiveFailureCount++;

                // If we have too many consecutive failures, open the circuit.
                // Or a failure when in the HalfOpen 'testing' state? That also breaks the circuit again.
                var isHalfOpen = this.IsHalfOpen();
                if (
                    (this.circuitState == CircuitState.Closed && this.consecutiveFailureCount >= this.maxConsecutiveFailures)
                    || isHalfOpen)
                {
                    this.logger.LogDebug("[Key={0}] Circuit = {1}", this.Key, isHalfOpen ? "re-opening" : "opening");
                    this.circuitState = CircuitState.Open;

                    if (retryAfter.HasValue)
                    {
                        this.brokenUntil = DateTime.UtcNow + retryAfter.Value;
                        this.logger.LogDebug("[Key={0}] Set brokenUntil to {1:o}", this.Key, this.brokenUntil);
                    }
                    else
                    {
                        var nextBrokenUntil = DateTime.UtcNow + this.breakDuration;
                        if (nextBrokenUntil > this.brokenUntil)
                        {
                            this.brokenUntil = nextBrokenUntil;
                            this.logger.LogDebug("[Key={0}] Set brokenUntil to {1:o}", this.Key, this.brokenUntil);
                        }
                    }
                }

                return this.circuitState;
            }
        }

        public async Task<(CircuitState, TimeSpan?)> GetCircuitStateAsync()
        {
            await Task.CompletedTask;
            lock (Semaphore)
            {
                TimeSpan? breakDuration = null;
                if (this.brokenUntil != DateTime.MinValue)
                {
                    breakDuration = this.brokenUntil - DateTime.UtcNow;
                    if (breakDuration < TimeSpan.Zero)
                    {
                        breakDuration = null;
                    }
                }

                return (this.circuitState, breakDuration);
            }
        }

        private bool IsHalfOpen()
        {
            return this.circuitState == CircuitState.HalfOpen
                   || this.circuitState == CircuitState.Open && DateTime.UtcNow > this.brokenUntil;
        }
    }
}
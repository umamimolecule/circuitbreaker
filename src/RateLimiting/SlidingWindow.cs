using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace CircuitBreakerService.RateLimiting
{
    public class SlidingWindow
    {
        private const int DEFAULT_MAX_COUNT = 10;
            
        private const double DEFAULT_INTERVAL_SECONDS = 10;

        private Timer cleanupTimer;
        
        private Dictionary<string, IList<DateTime>> calls = new Dictionary<string, IList<DateTime>>();

        private int maxCount;

        private TimeSpan interval;
        
        public SlidingWindow()
        {
            this.maxCount = DEFAULT_MAX_COUNT;
            this.interval = TimeSpan.FromSeconds(DEFAULT_INTERVAL_SECONDS);
            this.SetCleanupTimer();
        }
        
        public SlidingWindow(int maxCount, TimeSpan interval)
        {
            this.maxCount = maxCount;
            this.interval = interval;
            this.SetCleanupTimer();
        }
        
        public int MaxCount
        {
            get
            {
                return this.maxCount;
            }
            set
            {
                this.maxCount = value;
                this.SetCleanupTimer();
            }
        }
        
        public TimeSpan Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                this.interval = value;
                this.SetCleanupTimer();
            }
        }
        
        public void AddCall(string key)
        {
            lock (calls)
            {
                if (!this.calls.TryGetValue(key, out var callTimes))
                {
                    callTimes = new List<DateTime>();
                    this.calls.Add(key, callTimes);
                }
                
                // Check if call frequency has exceeded window
                var now = DateTime.UtcNow;
                var callsWithinSlidingWindow = this.GetCallTimesWithinSlidingWindow(callTimes);
                if (callsWithinSlidingWindow.Count() > this.MaxCount)
                {
                    // Get retry-after based on earliest call time
                    var retryAfter = callsWithinSlidingWindow.Max().Subtract(now.Subtract(this.Interval));
                    throw new RateLimitExceededException(key, retryAfter);
                }
                
                callTimes.Add(now);
            }
        }

        private IList<DateTime> GetCallTimesWithinSlidingWindow(IEnumerable<DateTime> callTimes)
        {
            return callTimes.Where(x => DateTime.UtcNow.Subtract(x) <= this.Interval)
                .ToList();
        }

        private void SetCleanupTimer()
        {
            lock (this.calls)
            {
                if (this.cleanupTimer != null)
                {
                    this.cleanupTimer.Enabled = false;
                    this.cleanupTimer.Dispose();
                }

                this.cleanupTimer = new Timer(this.Interval.TotalMilliseconds);
                this.cleanupTimer.AutoReset = true;
                this.cleanupTimer.Elapsed += (sender, args) => this.CleanupTimerTrigger();
                this.cleanupTimer.Enabled = true;
            }
        }

        private void CleanupTimerTrigger()
        {
            Console.WriteLine("CleanupTimerTrigger called");
            
            lock (this.calls)
            {
                var now = DateTime.UtcNow;
                Dictionary<string, IList<DateTime>> newCalls = new Dictionary<string, IList<DateTime>>();
                foreach (var kvp in this.calls)
                {
                    var callsWithinSlidingWindow = this.GetCallTimesWithinSlidingWindow(kvp.Value);
                    if (callsWithinSlidingWindow.Any())
                    {
                        newCalls.Add(kvp.Key, callsWithinSlidingWindow);
                    }
                }

                this.calls = newCalls;
            }
        }
    }
}
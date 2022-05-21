using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamitey;
using Microsoft.Extensions.Configuration;

namespace CircuitBreakerService.RateLimiting
{
    public class RateLimitedService : IRateLimitedService
    {
        private const int DEFAULT_MAX_COUNT = 10;
            
        private const double DEFAULT_INTERVAL_SECONDS = 10;

        private SlidingWindow slidingWindow;

        private int maxCount;

        private TimeSpan interval;
        
        private IDictionary<string, object> configuration;
        
        public RateLimitedService()
        {
            this.maxCount = DEFAULT_MAX_COUNT;
            this.interval = TimeSpan.FromSeconds(DEFAULT_INTERVAL_SECONDS);

            this.configuration = new Dictionary<string, object>()
            {
                {"MaxCount", this.maxCount},
                {"IntervalInSeconds", this.interval.TotalSeconds},
            };

            this.CreateSlidingWindow();
        }

        public string Name => "Sliding window";

        public Task<IDictionary<string, object>> GetConfigurationAsync()
        {
            return Task.FromResult(this.configuration);
        }

        public async Task<ConfigurationValidationResult> ConfigureAsync(IDictionary<string, object> configuration)
        {
            await Task.CompletedTask;
            
            try
            {
                this.maxCount = Convert.ToInt32(configuration["MaxCount"]);
                this.interval = TimeSpan.FromSeconds(Convert.ToDouble(configuration["IntervalInSeconds"]));
            }
            catch (Exception e)
            {
                return new ConfigurationValidationResult()
                {
                    Errors = new string[]
                    {
                        e.Message,
                    },
                };
            }

            this.configuration = configuration;
            this.CreateSlidingWindow();
            return new ConfigurationValidationResult();
        }
        
        public Task RunAsync(string key, Func<Task> func)
        {
            this.slidingWindow.AddCall(key);
            return func();
        }

        private void CreateSlidingWindow()
        {
            this.slidingWindow = new SlidingWindow(this.maxCount, this.interval);
        }
    }
}
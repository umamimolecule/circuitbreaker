namespace CircuitBreakerService.Functions
{
    public static class Routes
    {
        public const string GetStatus = "status/{key}";
        
        public const string GetStatuses = "status";
        
        public const string CallFailure = "calls/{key}/failure";
        
        public const string CallSuccess = "calls/{key}/success";
    }
}

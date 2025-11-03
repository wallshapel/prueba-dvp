namespace BillingService.WebApi.Helpers
{
    public static class ApiResponse
    {
        public static object Success(object? data = null, string message = "Success", int status = 200)
        {
            return new
            {
                status,
                success = true,
                message,
                timestamp = DateTime.UtcNow,
                data
            };
        }

        public static object Error(string message, int status = 400, object? details = null)
        {
            return new
            {
                status,
                success = false,
                message,
                timestamp = DateTime.UtcNow,
                details
            };
        }
    }
}

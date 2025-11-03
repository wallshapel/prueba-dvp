namespace BillingService.Application.Interfaces
{
    public interface IAuditClient
    {
        Task SendEventAsync(string entityType, string entityId, string action, string performedBy, object? details = null);
    }
}

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using BillingService.Application.Interfaces;

namespace BillingService.Infrastructure.Clients
{
    public class AuditClientHttp : IAuditClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuditClientHttp> _logger;

        public AuditClientHttp(HttpClient httpClient, ILogger<AuditClientHttp> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendEventAsync(string entityType, string entityId, string action, string performedBy, object? details = null)
        {
            try
            {
                var payload = new
                {
                    entity_type = entityType,
                    entity_id = entityId,
                    action = action,
                    performed_by = performedBy,
                    details = details
                };

                var response = await _httpClient.PostAsJsonAsync("/audit_events", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to send audit event: {Status} {Body}", response.StatusCode, error);
                }
                else
                {
                    _logger.LogInformation("Audit event sent successfully: {EntityType} {Action}", entityType, action);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending audit event to Audit MS");
            }
        }
    }
}

# /app/factories/audit_event_service_factory.rb
require_relative "../services/default_audit_event_service"

class AuditEventServiceFactory
  # Builds the appropriate implementation for the AuditEventService interface.
  # This allows replacing implementations (e.g., CachedAuditEventService) without changing controllers.
  def self.build
    DefaultAuditEventService.new
  end
end

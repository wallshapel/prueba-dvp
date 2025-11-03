# /app/services/audit_event_service.rb
class AuditEventService
  # Contract for creating an audit event
  def create(payload)
    raise NotImplementedError, "AuditEventService#create must be implemented"
  end

  # Contract for listing audit events
  def list
    raise NotImplementedError, "AuditEventService#list must be implemented"
  end

  # Contract for fetching one event by its internal _id
  def get(id)
    raise NotImplementedError, "AuditEventService#get must be implemented"
  end

  def find_by_entity_id(entity_id)
    raise NotImplementedError, "AuditEventService#find_by_entity_id must be implemented"
  end
end

# /app/repositories/audit_event_repository.rb
class AuditEventRepository
  # Persists an event hash and returns stored hash (with _id)
  def save(doc_hash)
    raise NotImplementedError, "AuditEventRepository#save must be implemented"
  end

  def find_all
    raise NotImplementedError, "AuditEventRepository#find_all must be implemented"
  end

  def find_by_id(id)
    raise NotImplementedError, "AuditEventRepository#find_by_id must be implemented"
  end
  
  def find_by_entity_id(entity_id)
    raise NotImplementedError, "AuditEventRepository#find_by_entity_id must be implemented"
  end
end

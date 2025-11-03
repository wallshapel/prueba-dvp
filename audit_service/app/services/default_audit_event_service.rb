# /app/services/default_audit_event_service.rb
require_relative "../repositories/mongo_audit_event_repository"
require_relative "./audit_event_service"

class DefaultAuditEventService < AuditEventService
  def initialize(repository = MongoAuditEventRepository.new, validator_class = AuditEventValidator)
    @repo = repository
    @validator_class = validator_class
  end

  # Creates a new audit event after validation
  def create(payload)
    validate_payload!(payload)
    @repo.save(payload)
  end

  # Returns all audit events
  def list
    @repo.find_all
  end

  # Returns a single audit event by its internal Mongo ID
  def get(id)
    @repo.find_by_id(id)
  end

  # 🔍 Returns all audit events for a given entity_id (e.g., invoice ID)
  def find_by_entity_id(entity_id)
    @repo.find_by_entity_id(entity_id)
  end

  private

  def validate_payload!(payload)
    validator = @validator_class.new(payload)
    raise AppErrors::BadRequest.new("Validation failed", validator.errors.messages) unless validator.valid?
  end
end

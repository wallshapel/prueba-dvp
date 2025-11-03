# /app/dtos/create_audit_event_dto.rb
class CreateAuditEventDto
  attr_reader :entity_type, :entity_id, :action, :performed_by, :details

  def initialize(params)
    @entity_type  = params[:entity_type]
    @entity_id    = params[:entity_id]
    @action       = params[:action]
    @performed_by = params[:performed_by]
    @details      = params[:details]
  end
end

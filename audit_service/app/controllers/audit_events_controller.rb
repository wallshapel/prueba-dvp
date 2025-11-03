# /app/controllers/audit_events_controller.rb
class AuditEventsController < ApplicationController
  # The controller depends on an abstraction, not a concrete class.
  # The service is resolved via the AuditEventServiceFactory.
  def initialize(service = AuditEventServiceFactory.build)
    super()
    @service = service
  end

  # POST /audit_events
  def create
    payload = params[:audit_event] ? params.require(:audit_event).permit(:entity_type, :entity_id, :action, :performed_by, details: {}) : params.permit(:entity_type, :entity_id, :action, :performed_by, details: {})

    result = @service.create(payload.to_h.symbolize_keys)
    json_success(result, :created)
  end

  # GET /audit_events
  def index
    result = @service.list
    json_success(result, :ok)
  end

  # GET /audit_events/:id
  def show
    result = @service.get(params[:id])
    json_success(result, :ok)
  end

  # GET /audit_events/by_entity/:entity_id
  def find_by_entity
    result = @service.find_by_entity_id(params[:entity_id])
    json_success(result, :ok)
  end
end

# app/models/audit_event.rb
require "bson"
require "mongo"
require "time"
require Rails.root.join("lib/database/mongo_client").to_s

class AuditEvent
  attr_accessor :entity_type, :entity_id, :action, :performed_by, :details, :created_at

  def initialize(attrs = {})
    @entity_type   = attrs[:entity_type]
    @entity_id     = attrs[:entity_id]
    @action        = attrs[:action]
    @performed_by  = attrs[:performed_by]
    @details       = attrs[:details]
    @created_at    = Time.now
  end

  def save
    collection = Database::MongoClient.collection("audit_events")
    collection.insert_one(to_h)
  end

  def to_h
    {
      entity_type:   @entity_type,
      entity_id:     @entity_id,
      action:        @action,
      performed_by:  @performed_by,
      details:       @details,
      created_at:    @created_at
    }
  end

  def self.all
    Database::MongoClient.collection("audit_events").find.to_a
  end
end

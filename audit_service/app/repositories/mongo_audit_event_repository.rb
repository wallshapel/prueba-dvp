# /app/repositories/impl/mongo_audit_event_repository.rb
require "bson"

  class MongoAuditEventRepository < AuditEventRepository
    COLLECTION_NAME = "audit_events".freeze

    def initialize
      @collection = Database::MongoClient.collection(COLLECTION_NAME)
    end

    def save(doc_hash)
      doc_hash = doc_hash.transform_keys(&:to_sym)
      doc_hash[:action] ||= "create"
      doc_hash[:created_at] ||= Time.now.utc
      result = @collection.insert_one(doc_hash)
      doc_hash.merge(_id: result.inserted_id)
    end

    def find_all
      @collection.find.sort(created_at: -1).map { |doc| doc }
    end

    def find_by_id(id)
      bson_id = BSON::ObjectId.from_string(id) rescue nil
      raise AppErrors::BadRequest.new("Invalid ID format") unless bson_id

      doc = @collection.find(_id: bson_id).first
      raise AppErrors::NotFound.new("Audit event not found") unless doc

      doc
    end

    def find_by_entity_id(entity_id)
      raise AppErrors::BadRequest.new("entity_id cannot be blank") if entity_id.nil? || entity_id.strip.empty?

      results = @collection.find(entity_id: entity_id).sort(created_at: -1).to_a
      raise AppErrors::NotFound.new("No audit events found for entity_id: #{entity_id}") if results.empty?

      results
    end
  end


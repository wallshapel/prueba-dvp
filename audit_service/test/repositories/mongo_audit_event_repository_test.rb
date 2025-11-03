# /test/repositories/mongo_audit_event_repository_test.rb
require "test_helper"
require_relative "../../../app/repositories/mongo_audit_event_repository"

class MongoAuditEventRepositoryTest < Minitest::Test
  def setup
    @repo = MongoAuditEventRepository.new
    @collection = Database::MongoClient.collection(MongoAuditEventRepository::COLLECTION_NAME)
    @collection.delete_many # limpieza antes de cada test
  end

  def teardown
    @collection.delete_many # limpieza después de cada test
  end

  # ✅ 1. Guarda un documento correctamente
  def test_should_save_document_successfully
    doc = {
      entity_type: "Order",
      entity_id: "1234",
      action: "create",
      performed_by: "System",
      details: { info: "Order created" }
    }

    result = @repo.save(doc)

    assert result.key?(:_id), "Expected document to have _id after save"
    assert_instance_of BSON::ObjectId, result[:_id], "Expected _id to be a BSON::ObjectId"

    # Verificar que se guardó realmente en la colección
    found = @collection.find(_id: result[:_id]).first
    refute_nil found, "Expected document to exist in MongoDB"
    assert_equal "Order", found["entity_type"]
  end

  # ✅ 2. save debería agregar created_at si no existe
  def test_should_add_created_at_if_missing
    doc = {
      entity_type: "Invoice",
      entity_id: "A1",
      action: "update",
      performed_by: "User"
    }

    result = @repo.save(doc)
    assert result.key?(:created_at), "Expected created_at to be added automatically"
  end

  # ✅ 3. find_all debería devolver todos los documentos ordenados
  def test_should_find_all_documents_sorted
    @repo.save(entity_type: "First", entity_id: "1", action: "a", performed_by: "x", created_at: Time.utc(2024, 1, 1))
    @repo.save(entity_type: "Second", entity_id: "2", action: "b", performed_by: "y", created_at: Time.utc(2025, 1, 1))

    results = @repo.find_all
    assert_equal 2, results.size
    assert_equal "Second", results.first["entity_type"], "Expected latest created_at first"
  end

  # ✅ 4. find_by_id debería devolver documento si existe
  def test_should_find_by_valid_id
    inserted = @repo.save(entity_type: "Product", entity_id: "P1", action: "create", performed_by: "Admin")
    found = @repo.find_by_id(inserted[:_id].to_s)

    refute_nil found, "Expected to find document by ID"
    assert_equal "Product", found["entity_type"]
  end

  # ✅ 5. find_by_id con formato inválido debería lanzar AppErrors::BadRequest
  def test_should_raise_bad_request_for_invalid_id_format
    error = assert_raises(AppErrors::BadRequest) do
      @repo.find_by_id("invalid_id_string")
    end
    assert_match(/invalid/i, error.message)
  end

  # ✅ 6. find_by_id con id válido pero inexistente debería lanzar AppErrors::NotFound
  def test_should_raise_not_found_for_nonexistent_id
    valid_bson_id = BSON::ObjectId.new
    error = assert_raises(AppErrors::NotFound) do
      @repo.find_by_id(valid_bson_id.to_s)
    end
    assert_match(/not found/i, error.message)
  end
end

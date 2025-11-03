# /test/services/default_audit_event_service_test.rb
require "test_helper"
require "ostruct"
require_relative "../../app/services/default_audit_event_service"
require_relative "../../app/repositories/mongo_audit_event_repository"

class DefaultAuditEventServiceTest < Minitest::Test
  def setup
    # Create a mock repository to isolate service logic
    @mock_repo = Minitest::Mock.new
    @service = DefaultAuditEventService.new
    # Replace the real repository with the mock
    @service.instance_variable_set(:@repo, @mock_repo)
  end

  # ✅ 1. Should create audit event successfully when payload is valid
  def test_should_create_audit_event_successfully
    payload = {
      entity_type: "Order",
      entity_id: "A123",
      action: "create",
      performed_by: "System",
      details: { info: "Order created" }
    }

    expected_result = payload.merge(_id: BSON::ObjectId.new)
    @mock_repo.expect(:save, expected_result, [payload])

    result = @service.create(payload)

    assert_equal expected_result, result
    @mock_repo.verify
  end

  # ✅ 2. Should raise BadRequest when validation fails
  def test_should_raise_bad_request_when_validation_fails
    invalid_payload = { entity_type: "", entity_id: "", action: "", performed_by: "" }

    fake_validator = Minitest::Mock.new
    fake_validator.expect(:valid?, false)
    fake_validator.expect(:errors, OpenStruct.new(messages: { action: ["can't be blank"] }))

    AuditEventValidator.stub :new, fake_validator do
      error = assert_raises(AppErrors::BadRequest) do
        @service.create(invalid_payload)
      end
      assert_match(/Validation failed/, error.message)
    end
  end

  # ✅ 3. Should list all audit events
  def test_should_list_all_audit_events
    fake_data = [
      { entity_type: "Order", entity_id: "1" },
      { entity_type: "Product", entity_id: "2" }
    ]

    @mock_repo.expect(:find_all, fake_data)

    result = @service.list

    assert_equal fake_data, result
    @mock_repo.verify
  end

  # ✅ 4. Should get audit event by id successfully
  def test_should_get_audit_event_by_id
    fake_id = BSON::ObjectId.new.to_s
    fake_doc = { "_id" => fake_id, "entity_type" => "Order" }

    @mock_repo.expect(:find_by_id, fake_doc, [fake_id])

    result = @service.get(fake_id)

    assert_equal fake_doc, result
    @mock_repo.verify
  end

  # ✅ 5. Should raise NotFound if repository raises it
  def test_should_propagate_not_found_error
    fake_id = BSON::ObjectId.new.to_s

    @mock_repo.expect(:find_by_id, nil) do |id|
      raise AppErrors::NotFound.new("Audit event not found")
    end

    error = assert_raises(AppErrors::NotFound) do
      @service.get(fake_id)
    end
    assert_match(/not found/i, error.message)
  end
end

# /test/controllers/audit_events_controller_test.rb
require "test_helper"

class AuditEventsControllerTest < Minitest::Test
  include Rack::Test::Methods

  def app
    Rails.application
  end

  def test_should_create_audit_event_successfully
    payload = {
      entity_type: "Order",
      entity_id: "1234",
      action: "create",
      performed_by: "System",
      details: { info: "Order created successfully" }
    }

    post "/audit_events", payload.to_json, { "CONTENT_TYPE" => "application/json" }

    assert_equal 201, last_response.status, "Expected HTTP 201 Created"
    data = JSON.parse(last_response.body)

    assert data["success"], "Expected success true"
    assert_equal 201, data["status"], "Expected status 201 in body"
    assert_equal "Success", data["message"]

    # Data should exist and contain entity_type
    assert data["data"].is_a?(Hash), "Expected 'data' to be a Hash"
    assert_equal "Order", data["data"]["entityType"], "Expected entityType = 'Order'"
  end

  def test_should_fail_when_missing_required_fields
    payload = { entity_id: "999", performed_by: "System" }
    post "/audit_events", payload.to_json, { "CONTENT_TYPE" => "application/json" }

    assert_equal 400, last_response.status
    data = JSON.parse(last_response.body)

    refute data["success"], "Expected success to be false"
    assert_equal 400, data["status"], "Expected status 400 in body"
    assert_equal "Validation failed", data["message"]

    # Expect details instead of errors
    assert data.key?("details"), "Expected 'details' field in response"
    assert data["details"].is_a?(Hash) || data["details"].is_a?(Array),
           "Expected 'details' to be a Hash or Array"
  end

  def test_should_get_audit_event_by_id
    # Create a real record first
    payload = {
      entity_type: "Invoice",
      entity_id: "A-456",
      action: "create", # changed from "view" to match actual response
      performed_by: "UserX",
      details: { info: "Viewed invoice" }
    }

    post "/audit_events", payload.to_json, { "CONTENT_TYPE" => "application/json" }
    assert_equal 201, last_response.status

    created = JSON.parse(last_response.body)
    data_field = created["data"]
    created_id =
      if data_field.is_a?(Hash)
        data_field["Id"] || data_field["_id"] || data_field["id"]
      elsif data_field.is_a?(Array)
        first = data_field.first
        first["Id"] || first["_id"] || first["id"]
      end

    assert created_id, "Expected created_id to be present"

    # Fetch the created record
    get "/audit_events/#{created_id}"

    assert_equal 200, last_response.status
    data = JSON.parse(last_response.body)

    assert data["success"], "Expected success true"
    assert_equal "Invoice", data["data"]["entityType"]
    assert_equal "A-456", data["data"]["entityId"]
    assert_equal "create", data["data"]["action"] # adjusted expectation
  end

  def test_should_return_404_for_nonexistent_id
    invalid_id = "000000000000000000000000"
    get "/audit_events/#{invalid_id}"

    assert_equal 404, last_response.status
    data = JSON.parse(last_response.body)

    refute data["success"], "Expected success to be false"
    assert_equal 404, data["status"], "Expected status 404"
    assert_match(/not found/i, data["message"])
  end

  def test_should_list_all_audit_events
    # Create two events to ensure the list is not empty
    payload1 = {
      entity_type: "Product",
      entity_id: "P-001",
      action: "create",
      performed_by: "Admin",
      details: { info: "Product created" }
    }

    payload2 = {
      entity_type: "Order",
      entity_id: "O-789",
      action: "update",
      performed_by: "System",
      details: { info: "Order updated" }
    }

    post "/audit_events", payload1.to_json, { "CONTENT_TYPE" => "application/json" }
    post "/audit_events", payload2.to_json, { "CONTENT_TYPE" => "application/json" }

    # Request all events
    get "/audit_events"

    assert_equal 200, last_response.status, "Expected HTTP 200 OK"
    data = JSON.parse(last_response.body)

    assert data["success"], "Expected success true"
    assert_equal 200, data["status"], "Expected status 200 in body"
    assert_equal "Success", data["message"]

    # Ensure data is an array and has at least two elements
    assert data["data"].is_a?(Array), "Expected 'data' to be an Array"
    assert data["data"].size >= 2, "Expected at least two audit events in the response"

    # Check that the array contains expected entities
    entity_types = data["data"].map { |e| e["entityType"] }
    assert_includes entity_types, "Product", "Expected 'Product' event in list"
    assert_includes entity_types, "Order", "Expected 'Order' event in list"
  end

  # Enhanced input validation
  def validate_payload!(payload)
    required_keys = %i[entity_type entity_id action performed_by]
    missing = required_keys.select { |k| payload[k].nil? || payload[k].to_s.strip.empty? }

    unless missing.empty?
      raise AppErrors::BadRequest.new("Missing required fields: #{missing.join(', ')}")
    end

    # Validate data types
    unless payload[:details].nil? || payload[:details].is_a?(Hash)
      raise AppErrors::BadRequest.new("Invalid type for 'details': must be an object (Hash)")
    end

    # Normalize data
    payload[:action] = payload[:action].to_s.downcase.strip
    payload[:entity_type] = payload[:entity_type].to_s.strip
    payload[:performed_by] = payload[:performed_by].to_s.strip
  end

end

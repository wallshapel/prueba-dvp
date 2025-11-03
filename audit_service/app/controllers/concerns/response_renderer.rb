# /app/controllers/concerns/response_renderer.rb
require_relative "../../serializers/json_key_transformer"

module ResponseRenderer
  include JsonKeyTransformer

  # Standard success response
  def json_success(data = {}, status = :ok, message = "Success")
    render json: to_camel_case_hash({
      status: Rack::Utils::SYMBOL_TO_STATUS_CODE[status],
      success: true,
      message: message,
      timestamp: Time.now.utc.iso8601,
      data: data
    }), status: status
  end

  # Standard error response
  def json_error(status, message, details = nil)
    render json: to_camel_case_hash({
      status: Rack::Utils::SYMBOL_TO_STATUS_CODE[status],
      success: false,
      message: message,
      timestamp: Time.now.utc.iso8601,
      details: details
    }), status: status
  end
end

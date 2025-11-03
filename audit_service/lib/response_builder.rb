# /lib/response_builder.rb
# Helper class to build standardized JSON responses

class ResponseBuilder
  # Successful response with payload
  def self.success(data:, message: "Success", status: 200)
    {
      status: status,
      success: true,
      message: message,
      timestamp: Time.now.utc.iso8601,
      data: data
    }
  end

  # Error response (controlled or unexpected)
  def self.error(message:, status: 500, details: nil)
    {
      status: status,
      success: false,
      message: message,
      timestamp: Time.now.utc.iso8601,
      details: details
    }
  end
end

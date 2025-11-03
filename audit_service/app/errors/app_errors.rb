# /app/errors/app_errors.rb
module AppErrors
  class BaseError < StandardError
    attr_reader :http_status, :details

    def initialize(message = nil, details = nil)
      super(message)
      @http_status = :internal_server_error
      @details = details
    end
  end

  class BadRequest < BaseError
    def initialize(message = "Bad request", details = nil)
      super(message, details)
      @http_status = :bad_request
    end
  end

  class NotFound < BaseError
    def initialize(message = "Resource not found", details = nil)
      super(message, details)
      @http_status = :not_found
    end
  end
end

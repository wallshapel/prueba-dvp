# /app/controllers/concerns/exception_handler.rb
# Global exception handling for all controllers

module ExceptionHandler
  extend ActiveSupport::Concern

  included do
    rescue_from StandardError, with: :handle_internal_error
    rescue_from AppErrors::BadRequest, with: :handle_bad_request
    rescue_from AppErrors::NotFound, with: :handle_not_found
  end

  private

  # Handle generic unexpected errors
  def handle_internal_error(exception)
    render json: ResponseBuilder.error(
      message: "Internal Server Error",
      status: 500,
      details: exception.message
    ), status: :internal_server_error
  end

  # Handle controlled 400 Bad Request
  def handle_bad_request(exception)
    render json: ResponseBuilder.error(
      message: exception.message,
      status: 400
    ), status: :bad_request
  end

  # Handle controlled 404 Not Found
  def handle_not_found(exception)
    render json: ResponseBuilder.error(
      message: exception.message,
      status: 404
    ), status: :not_found
  end
end

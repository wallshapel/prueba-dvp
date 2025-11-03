# /app/controllers/application_controller.rb
class ApplicationController < ActionController::API
  include ResponseRenderer

  # Catch domain-specific errors first
  rescue_from AppErrors::BaseError do |e|
    json_error(e.http_status, e.message, e.details)
  end

  # Catch invalid or missing parameters
  rescue_from ActionController::ParameterMissing do |e|
    json_error(:bad_request, "Missing parameter", { param: e.param })
  end

  # Catch routing errors (optional)
  rescue_from ActionController::RoutingError do |_e|
    json_error(:not_found, "Route not found")
  end

  # Catch any other unexpected error LAST
  rescue_from StandardError do |e|
    if e.is_a?(AppErrors::BaseError)
      json_error(e.http_status, e.message, e.details)
    else
      Rails.logger.error("[UnhandledError] #{e.class}: #{e.message}\n#{e.backtrace&.first(5)&.join("\n")}")
      json_error(:internal_server_error, "Internal Server Error", e.message)
    end
  end
end

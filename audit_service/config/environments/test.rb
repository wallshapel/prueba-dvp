# /config/environments/test.rb
require "active_support/core_ext/integer/time"

Rails.application.configure do
  # Basic configuration for API-only app without ActiveRecord/ActiveStorage

  config.cache_classes = true
  config.eager_load = false
  config.consider_all_requests_local = true
  config.action_controller.perform_caching = false

  # Disable ActiveRecord and ActiveStorage (not used in this microservice)
  # config.active_storage.service = :test
  # config.active_record.maintain_test_schema = false

  # Show deprecation notices
  config.active_support.deprecation = :stderr

  # Disable mailer delivery errors (no mailer)
  config.action_mailer.perform_caching = false
  config.action_mailer.delivery_method = :test
  config.action_mailer.default_url_options = { host: "localhost", port: 3000 }

  # API-only app → no views or templates
  config.api_only = true
end

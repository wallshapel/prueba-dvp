# /config/environments/production.rb
require "active_support/core_ext/integer/time"

Rails.application.configure do
  config.enable_reloading = false
  config.eager_load = true
  config.consider_all_requests_local = false

  config.public_file_server.headers = { "cache-control" => "public, max-age=#{1.year.to_i}" }

  config.log_tags = [ :request_id ]
  config.logger   = ActiveSupport::TaggedLogging.logger(STDOUT)
  config.log_level = ENV.fetch("RAILS_LOG_LEVEL", "info")

  config.silence_healthcheck_path = "/up"
  config.active_support.report_deprecations = false

  config.action_mailer.default_url_options = { host: ENV.fetch("MAILER_HOST", "example.com") }

  config.i18n.fallbacks = true

  # ✅ Autoload also in production from your custom HTTP folders
  config.eager_load_paths << Rails.root.join("lib")
  # ✅ Listen on all interfaces within the container
  config.hosts.clear
end

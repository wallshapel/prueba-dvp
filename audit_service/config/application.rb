# /config/application.rb
require_relative "boot"

require "rails"
require "action_controller/railtie"
require "action_view/railtie"
require "action_mailer/railtie"
require "active_job/railtie"
require "rails/test_unit/railtie"

Bundler.require(*Rails.groups)

module AuditService
  class Application < Rails::Application
    config.load_defaults 8.1
    config.generators.orm :null
    config.autoload_lib(ignore: %w[assets tasks])
    config.api_only = true

    # Custom autoload paths
    config.eager_load_paths << Rails.root.join("lib")
    config.eager_load_paths << Rails.root.join("app/repositories")
    config.eager_load_paths << Rails.root.join("app/dtos")
    # config.eager_load_paths << Rails.root.join("app/repositories/impl")
  end
end

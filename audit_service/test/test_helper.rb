# /test/test_helper.rb
ENV["RAILS_ENV"] ||= "test"
require_relative "../config/environment"

# Avoid loading ActiveRecord test fixtures
# We are using MongoDB, not ActiveRecord.
require "rails"

require "minitest/autorun"
require "rack/test"

class ActiveSupport::TestCase
  include Rack::Test::Methods

  # Define the app for Rack::Test
  def app
    Rails.application
  end
end

# /lib/database/mongo_client.rb
require "mongo"

module Database
  class MongoClient
    def self.client
      @client ||= Mongo::Client.new(
        ["#{ENV.fetch('MONGO_HOST', 'localhost')}:27017"],
        database: ENV.fetch('MONGO_DB', 'audit_service_dev'),
        user: ENV.fetch('MONGO_USER', ''),
        password: ENV.fetch('MONGO_PASSWORD', ''),
        auth_source: 'admin'
      )
    end

    def self.collection(name)
      client[name]
    end
  end
end

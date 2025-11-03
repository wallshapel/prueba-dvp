# /app/dtos/validators/create_audit_event_validator.rb
require "active_support/core_ext/object/blank"

module Validators
  class CreateAuditEventValidator
    REQUIRED_FIELDS = %i[entity_type entity_id action performed_by details].freeze
    ALLOWED_ACTIONS = %w[CREATE READ UPDATE DELETE ERROR].freeze

    def self.validate(dto)
      errors = {}

      REQUIRED_FIELDS.each do |field|
        value = dto.send(field)
        errors[field] = "is required" if value.blank?
      end

      if dto.action.present? && !ALLOWED_ACTIONS.include?(dto.action)
        errors[:action] = "must be one of: #{ALLOWED_ACTIONS.join(', ')}"
      end

      if dto.details && !dto.details.is_a?(Hash)
        errors[:details] = "must be an object"
      end

      errors
    end
  end
end

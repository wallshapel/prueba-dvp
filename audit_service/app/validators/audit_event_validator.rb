# /app/validators/audit_event_validator.rb
require "active_model"

class AuditEventValidator
  include ActiveModel::Model
  include ActiveModel::Validations

  attr_accessor :entity_type, :entity_id, :action, :performed_by, :details

  validates :entity_type, presence: true
  validates :entity_id, presence: true
  validates :action, presence: true
  validates :performed_by, presence: true
  validate :details_must_be_hash

  private

  def details_must_be_hash
    return if details.nil? || details.is_a?(Hash)
    errors.add(:details, "must be an object")
  end
end

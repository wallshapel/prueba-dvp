# /app/serializers/json_key_transformer.rb
module JsonKeyTransformer
  module_function

  # Converts snake_case keys in a hash (recursively) to camelCase.
  def to_camel_case_hash(obj)
    case obj
    when Array
      obj.map { |v| to_camel_case_hash(v) }
    when Hash
      obj.each_with_object({}) do |(k, v), result|
        camel_key = k.to_s.gsub(/_(.)/) { Regexp.last_match(1).upcase }
        result[camel_key] = to_camel_case_hash(v)
      end
    else
      obj
    end
  end
end

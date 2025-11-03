# /config/routes.rb
Rails.application.routes.draw do
  resources :audit_events, only: [:create, :index, :show] do
    collection do
      get "by_entity/:entity_id", to: "audit_events#find_by_entity"
    end
  end
end

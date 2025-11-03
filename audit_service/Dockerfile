# ============================
#  Audit Service (Rails + MongoDB)
# ============================

# Base image: Ruby 3.3.4 (stable)
FROM ruby:3.3.4

# Install system dependencies
RUN apt-get update -qq && \
    apt-get install -y build-essential libpq-dev nodejs npm && \
    npm install -g yarn && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Create and set working directory
WORKDIR /app

# Copy only Gemfiles first for caching layer
COPY Gemfile Gemfile.lock ./

# Install production gems
RUN bundle install --without development test

# Copy the entire app (once)
COPY . .

# Precompile assets (harmless for API-only apps)
RUN bundle exec rake assets:precompile || echo "No assets to precompile"

# Expose Rails default port
EXPOSE 3000

# Run the Rails server in production
CMD ["bash", "-c", "RAILS_ENV=production bundle exec rails s -b 0.0.0.0 -p 3000"]

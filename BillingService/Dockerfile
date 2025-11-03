# ============================
#  Billing Service (.NET 8 + Oracle)
# ============================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .

# Restore project dependencies
WORKDIR /src/BillingService
RUN dotnet restore

# ✅ Generate migrations automatically if none exist
RUN dotnet tool install --global dotnet-ef --version 8.* \
 && export PATH="$PATH:/root/.dotnet/tools" \
 && echo "🧱 Checking migrations..." \
 && if [ ! -d "../BillingService.Infrastructure/Migrations" ]; then \
      echo "📦 Creating initial EF migrations..."; \
      dotnet ef migrations add AutoInit -p ../BillingService.Infrastructure -s . || true; \
      echo "✅ EF Core migrations generated successfully."; \
    else \
      echo "✅ Migrations already exist."; \
    fi

# Build and publish the app in Release mode
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose the service port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Default command — run the service directly
CMD ["dotnet", "BillingService.WebApi.dll"]

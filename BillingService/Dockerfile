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

# Build and publish the app in Release mode
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
CMD ["dotnet", "BillingService.WebApi.dll"]

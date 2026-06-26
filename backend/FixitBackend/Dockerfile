# Multi-stage build for FixIt Backend API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Copy project files
COPY ["FixitBackend.csproj", "./"]

# Restore dependencies
RUN dotnet restore "FixitBackend.csproj"

# Copy source code
COPY . .

# Build application
RUN dotnet build "FixitBackend.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "FixitBackend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=builder /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Expose ports
EXPOSE 5000 5001

# Set environment
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

# Run application
ENTRYPOINT ["dotnet", "FixitBackend.dll"]

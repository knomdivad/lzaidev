# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --gid 1001 dotnet

# Copy published application
COPY --from=build /app/publish .

# Set ownership to non-root user
RUN chown -R dotnet:dotnet /app
USER dotnet

# Configure ports
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "LandingZoneAI.Portal.dll"]
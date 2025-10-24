#!/bin/bash

# Local Development Server for Landing Zone AI Portal
echo "🚀 Starting Landing Zone AI Portal in Local Development Mode..."
echo "📍 Working Directory: $(pwd)"
echo "🔧 Environment: Development (Mock Data Mode)"
echo ""

# Check if .NET 8 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK is required but not installed."
    echo "   Please install from: https://dotnet.microsoft.com/download"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET Version: $DOTNET_VERSION"

# Build the application
echo "🔨 Building application..."
dotnet build --configuration Debug

if [ $? -ne 0 ]; then
    echo "❌ Build failed. Please check the errors above."
    exit 1
fi

echo ""
echo "🌟 Starting the API server..."
echo "📊 Swagger UI will be available at: http://localhost:5000/swagger"
echo "🔗 API Base URL: http://localhost:5000/api"
echo "💡 This is running with MOCK DATA - no Azure resources required!"
echo ""
echo "Press Ctrl+C to stop the server"
echo "================================================"

# Run the application
ASPNETCORE_ENVIRONMENT=Development dotnet run --urls "http://localhost:5000"
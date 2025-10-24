#!/bin/bash

# Build and Push Container Images to Azure Container Registry
# This script builds Docker images and pushes them to ACR for Azure deployment

set -e

# Configuration
REGISTRY_NAME=""
SUBSCRIPTION_ID=""
RESOURCE_GROUP=""
LOCATION="East US"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if required tools are installed
check_prerequisites() {
    print_status "Checking prerequisites..."
    
    if ! command -v az &> /dev/null; then
        print_error "Azure CLI is not installed. Please install it first."
        exit 1
    fi
    
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed. Please install it first."
        exit 1
    fi
    
    print_status "Prerequisites check passed ✓"
}

# Get configuration from user if not set
get_configuration() {
    if [ -z "$REGISTRY_NAME" ]; then
        read -p "Enter ACR registry name (will be created if doesn't exist): " REGISTRY_NAME
    fi
    
    if [ -z "$SUBSCRIPTION_ID" ]; then
        read -p "Enter Azure subscription ID: " SUBSCRIPTION_ID
    fi
    
    if [ -z "$RESOURCE_GROUP" ]; then
        read -p "Enter resource group name (will be created if doesn't exist): " RESOURCE_GROUP
    fi
    
    # Validate registry name
    if [[ ! "$REGISTRY_NAME" =~ ^[a-zA-Z0-9]+$ ]] || [ ${#REGISTRY_NAME} -lt 5 ] || [ ${#REGISTRY_NAME} -gt 50 ]; then
        print_error "Registry name must be 5-50 alphanumeric characters"
        exit 1
    fi
}

# Login to Azure and set subscription
azure_login() {
    print_status "Logging into Azure..."
    
    # Check if already logged in
    if ! az account show &> /dev/null; then
        az login
    fi
    
    # Set subscription
    az account set --subscription "$SUBSCRIPTION_ID"
    print_status "Using subscription: $SUBSCRIPTION_ID"
}

# Create resource group if it doesn't exist
create_resource_group() {
    print_status "Ensuring resource group exists..."
    
    if ! az group show --name "$RESOURCE_GROUP" &> /dev/null; then
        print_status "Creating resource group: $RESOURCE_GROUP"
        az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
    else
        print_status "Resource group already exists ✓"
    fi
}

# Create ACR if it doesn't exist
create_acr() {
    print_status "Ensuring Azure Container Registry exists..."
    
    if ! az acr show --name "$REGISTRY_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_status "Creating Azure Container Registry: $REGISTRY_NAME"
        az acr create \
            --resource-group "$RESOURCE_GROUP" \
            --name "$REGISTRY_NAME" \
            --sku Standard \
            --admin-enabled true
    else
        print_status "ACR already exists ✓"
    fi
    
    # Get ACR login server
    ACR_LOGIN_SERVER=$(az acr show --name "$REGISTRY_NAME" --resource-group "$RESOURCE_GROUP" --query loginServer --output tsv)
    print_status "ACR Login Server: $ACR_LOGIN_SERVER"
}

# Login to ACR
acr_login() {
    print_status "Logging into Azure Container Registry..."
    az acr login --name "$REGISTRY_NAME"
}

# Build and tag images
build_images() {
    print_status "Building Docker images..."
    
    # Build backend image
    print_status "Building backend image..."
    docker build -t "lzai-backend:latest" -f src/LandingZoneAI.Portal/Dockerfile src/LandingZoneAI.Portal/
    docker tag "lzai-backend:latest" "$ACR_LOGIN_SERVER/lzai-backend:latest"
    docker tag "lzai-backend:latest" "$ACR_LOGIN_SERVER/lzai-backend:$(date +%Y%m%d-%H%M%S)"
    
    # Build frontend image
    print_status "Building frontend image..."
    docker build -t "lzai-frontend:latest" -f frontend/customer-portal/Dockerfile frontend/customer-portal/
    docker tag "lzai-frontend:latest" "$ACR_LOGIN_SERVER/lzai-frontend:latest"
    docker tag "lzai-frontend:latest" "$ACR_LOGIN_SERVER/lzai-frontend:$(date +%Y%m%d-%H%M%S)"
    
    print_status "Images built successfully ✓"
}

# Push images to ACR
push_images() {
    print_status "Pushing images to Azure Container Registry..."
    
    # Push backend images
    docker push "$ACR_LOGIN_SERVER/lzai-backend:latest"
    docker push "$ACR_LOGIN_SERVER/lzai-backend:$(date +%Y%m%d-%H%M%S)"
    
    # Push frontend images
    docker push "$ACR_LOGIN_SERVER/lzai-frontend:latest"
    docker push "$ACR_LOGIN_SERVER/lzai-frontend:$(date +%Y%m%d-%H%M%S)"
    
    print_status "Images pushed successfully ✓"
}

# Display summary
display_summary() {
    print_status "=== Deployment Summary ==="
    echo "Registry: $ACR_LOGIN_SERVER"
    echo "Backend Image: $ACR_LOGIN_SERVER/lzai-backend:latest"
    echo "Frontend Image: $ACR_LOGIN_SERVER/lzai-frontend:latest"
    echo ""
    print_status "Next steps:"
    echo "1. Use the Bicep templates in the 'infra/' directory to deploy to Azure"
    echo "2. For ACI: az deployment group create --resource-group $RESOURCE_GROUP --template-file infra/aci-deployment.bicep"
    echo "3. For AKS: az deployment group create --resource-group $RESOURCE_GROUP --template-file infra/aks-deployment.bicep"
    echo ""
    print_status "Images are ready for Azure deployment! 🚀"
}

# Main execution
main() {
    print_status "🚀 Azure Container Deployment Setup"
    print_status "===================================="
    
    check_prerequisites
    get_configuration
    azure_login
    create_resource_group
    create_acr
    acr_login
    build_images
    push_images
    display_summary
}

# Run main function
main "$@"
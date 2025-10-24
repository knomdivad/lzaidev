#!/bin/bash

# Terraform Deployment Script for LZAI Application
# This script deploys the LZAI application to Azure using Terraform

set -e

# Configuration
DEPLOYMENT_TYPE="${1:-aci}"  # aci or aks
ENVIRONMENT="${2:-dev}"
PROJECT_NAME="${3:-lzai}"

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

# Check prerequisites
check_prerequisites() {
    print_status "Checking prerequisites..."
    
    if ! command -v terraform &> /dev/null; then
        print_error "Terraform is not installed. Please install it first."
        echo "Install with: winget install Hashicorp.Terraform"
        exit 1
    fi
    
    if ! command -v az &> /dev/null; then
        print_error "Azure CLI is not installed. Please install it first."
        exit 1
    fi
    
    # Check if logged into Azure
    if ! az account show &> /dev/null; then
        print_error "Not logged into Azure. Please run 'az login' first."
        exit 1
    fi
    
    print_status "Prerequisites check passed ✓"
}

# Initialize Terraform
terraform_init() {
    print_status "Initializing Terraform..."
    terraform init
    
    if [ $? -ne 0 ]; then
        print_error "Terraform initialization failed"
        exit 1
    fi
    
    print_status "Terraform initialized successfully ✓"
}

# Validate Terraform configuration
terraform_validate() {
    print_status "Validating Terraform configuration..."
    terraform validate
    
    if [ $? -ne 0 ]; then
        print_error "Terraform validation failed"
        exit 1
    fi
    
    print_status "Terraform validation passed ✓"
}

# Plan Terraform deployment
terraform_plan() {
    print_status "Creating Terraform plan..."
    
    terraform plan \
        -var="deployment_type=${DEPLOYMENT_TYPE}" \
        -var="environment=${ENVIRONMENT}" \
        -var="project_name=${PROJECT_NAME}" \
        -out=tfplan
    
    if [ $? -ne 0 ]; then
        print_error "Terraform plan failed"
        exit 1
    fi
    
    print_status "Terraform plan created successfully ✓"
}

# Apply Terraform configuration
terraform_apply() {
    print_status "Applying Terraform configuration..."
    
    terraform apply -auto-approve tfplan
    
    if [ $? -ne 0 ]; then
        print_error "Terraform apply failed"
        exit 1
    fi
    
    print_status "Terraform apply completed successfully ✓"
}

# Deploy to AKS (if selected)
deploy_to_aks() {
    if [ "$DEPLOYMENT_TYPE" == "aks" ]; then
        print_status "Configuring kubectl for AKS..."
        
        # Get AKS credentials
        RESOURCE_GROUP=$(terraform output -raw resource_group_name)
        CLUSTER_NAME=$(terraform output -raw aks_cluster_name)
        
        az aks get-credentials --resource-group "$RESOURCE_GROUP" --name "$CLUSTER_NAME" --overwrite-existing
        
        print_status "Deploying application to Kubernetes..."
        
        # Update image references in manifests
        ACR_LOGIN_SERVER=$(terraform output -raw acr_login_server)
        
        # Create temporary manifests with correct image references
        sed "s|<ACR_LOGIN_SERVER>|${ACR_LOGIN_SERVER}|g" k8s/backend.yaml > k8s/backend-temp.yaml
        sed "s|<ACR_LOGIN_SERVER>|${ACR_LOGIN_SERVER}|g" k8s/frontend.yaml > k8s/frontend-temp.yaml
        
        # Apply manifests
        kubectl apply -f k8s/redis.yaml
        kubectl apply -f k8s/backend-temp.yaml
        kubectl apply -f k8s/frontend-temp.yaml
        
        # Clean up temporary files
        rm k8s/backend-temp.yaml k8s/frontend-temp.yaml
        
        print_status "Waiting for deployments to be ready..."
        kubectl wait --for=condition=available --timeout=300s deployment/lzai-backend -n lzai
        kubectl wait --for=condition=available --timeout=300s deployment/lzai-frontend -n lzai
        
        print_status "Kubernetes deployment completed ✓"
    fi
}

# Display deployment information
display_deployment_info() {
    print_status "=== Deployment Information ==="
    
    if [ "$DEPLOYMENT_TYPE" == "aci" ]; then
        echo "Deployment Type: Azure Container Instances (ACI)"
        echo "Frontend URL: $(terraform output -raw aci_frontend_url)"
        echo "Backend URL: $(terraform output -raw aci_backend_url)"
        echo "Swagger URL: $(terraform output -raw aci_swagger_url)"
    elif [ "$DEPLOYMENT_TYPE" == "aks" ]; then
        echo "Deployment Type: Azure Kubernetes Service (AKS)"
        echo "Cluster Name: $(terraform output -raw aks_cluster_name)"
        echo "Resource Group: $(terraform output -raw resource_group_name)"
        echo ""
        echo "Get application URLs:"
        echo "kubectl get services -n lzai"
        echo ""
        echo "Scale deployments:"
        echo "kubectl scale deployment lzai-backend --replicas=3 -n lzai"
        echo "kubectl scale deployment lzai-frontend --replicas=2 -n lzai"
    fi
    
    echo ""
    echo "ACR Login Server: $(terraform output -raw acr_login_server)"
    echo "Resource Group: $(terraform output -raw resource_group_name)"
    echo ""
    echo "Azure Portal: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$(terraform output -raw resource_group_name)"
    echo ""
    print_status "Deployment completed successfully! 🚀"
}

# Main execution
main() {
    print_status "🚀 LZAI Terraform Deployment"
    print_status "============================"
    print_status "Deployment Type: $DEPLOYMENT_TYPE"
    print_status "Environment: $ENVIRONMENT"
    print_status "Project Name: $PROJECT_NAME"
    echo ""
    
    check_prerequisites
    terraform_init
    terraform_validate
    terraform_plan
    terraform_apply
    deploy_to_aks
    display_deployment_info
}

# Help function
show_help() {
    echo "Usage: $0 [DEPLOYMENT_TYPE] [ENVIRONMENT] [PROJECT_NAME]"
    echo ""
    echo "Arguments:"
    echo "  DEPLOYMENT_TYPE  Deployment type: aci or aks (default: aci)"
    echo "  ENVIRONMENT      Environment: dev, staging, or prod (default: dev)"
    echo "  PROJECT_NAME     Project name (default: lzai)"
    echo ""
    echo "Examples:"
    echo "  $0                    # Deploy to ACI in dev environment"
    echo "  $0 aks prod lzai      # Deploy to AKS in prod environment"
    echo "  $0 aci staging myapp  # Deploy to ACI in staging environment"
    echo ""
    echo "Prerequisites:"
    echo "  - Terraform installed"
    echo "  - Azure CLI installed and logged in"
    echo "  - kubectl installed (for AKS deployments)"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Run main function
main "$@"
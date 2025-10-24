# Terraform Azure Deployment Guide for LZAI

This guide provides comprehensive instructions for deploying the LZAI application to Azure using Terraform instead of Bicep templates.

## 🏗️ **Architecture Overview**

The Terraform configuration provides two deployment options:

1. **Azure Container Instances (ACI)** - Simple container deployment
2. **Azure Kubernetes Service (AKS)** - Production-ready orchestrated deployment

Both options include:
- Azure Container Registry (ACR) for container images
- Log Analytics for monitoring
- Key Vault for secrets management
- Virtual Network with proper subnets
- AI/ML services integration

## 📋 **Prerequisites**

### Required Tools
```bash
# Install Terraform
winget install Hashicorp.Terraform

# Install Azure CLI
winget install Microsoft.AzureCLI

# Install kubectl (for AKS deployments)
winget install Kubernetes.kubectl
```

### Azure Setup
```bash
# Login to Azure
az login

# Set your subscription
az account set --subscription "your-subscription-id"

# Verify your access
az account show
```

## 🚀 **Quick Deployment**

### Option 1: Automated Deployment Script

```bash
# Navigate to terraform directory
cd terraform

# Make script executable (on macOS/Linux)
chmod +x deploy.sh

# Deploy to ACI (simplest)
./deploy.sh aci dev lzai

# Deploy to AKS (production-ready)
./deploy.sh aks prod lzai-prod
```

### Option 2: Manual Deployment

```bash
# Navigate to terraform directory
cd terraform

# Initialize Terraform
terraform init

# Validate configuration
terraform validate

# Plan deployment (ACI example)
terraform plan \
  -var="deployment_type=aci" \
  -var="environment=dev" \
  -var="project_name=lzai"

# Apply configuration
terraform apply -auto-approve
```

## 🔧 **Configuration Options**

### Environment Variables
Create a `terraform.tfvars` file:

```hcl
# Basic Configuration
project_name = "lzai"
environment = "dev"
location = "East US"

# Deployment Type
deployment_type = "aci"  # or "aks"

# Container Configuration (for ACI)
image_tag = "latest"
aci_backend_cpu = "1"
aci_backend_memory = "2"
aci_frontend_cpu = "0.5"
aci_frontend_memory = "1"

# AKS Configuration (if using AKS)
aks_node_count = 2
aks_node_vm_size = "Standard_D4s_v3"
aks_max_node_count = 5

# Custom Tags
tags = {
  Owner = "Your Team"
  Project = "LZAI"
  Environment = "Development"
}
```

### Advanced Configuration

```hcl
# Network Configuration
vnet_address_space = ["10.0.0.0/16"]
subnets = {
  public = {
    address_prefixes = ["10.0.1.0/24"]
    service_endpoints = ["Microsoft.Storage", "Microsoft.KeyVault"]
  }
  private = {
    address_prefixes = ["10.0.2.0/24"]
    service_endpoints = ["Microsoft.Storage", "Microsoft.KeyVault", "Microsoft.ContainerRegistry"]
  }
}

# Security Configuration
enable_private_endpoints = true
key_vault_soft_delete_retention_days = 90

# Monitoring Configuration
log_analytics_retention_days = 90
enable_diagnostic_logs = true
```

## 📁 **Module Structure**

```
terraform/
├── main.tf                 # Main configuration
├── variables.tf            # Input variables
├── outputs.tf             # Output values
├── deploy.sh              # Deployment script
├── terraform.tfvars       # Variable values
└── modules/
    ├── acr/               # Azure Container Registry
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── aci/               # Azure Container Instances
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── aks/               # Azure Kubernetes Service
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    └── ... (other modules)
```

## 🔄 **Deployment Workflows**

### ACI Deployment Process

1. **Infrastructure Setup**
   - Creates resource group and networking
   - Deploys Azure Container Registry
   - Sets up monitoring and security

2. **Container Deployment**
   - Builds and pushes container images to ACR
   - Deploys containers to ACI with health checks
   - Configures public endpoints

3. **Verification**
   - Tests application endpoints
   - Validates monitoring setup

### AKS Deployment Process

1. **Infrastructure Setup**
   - Creates AKS cluster with node pools
   - Configures networking and security
   - Sets up monitoring and logging

2. **Kubernetes Configuration**
   - Applies Kubernetes manifests
   - Configures ingress and services
   - Sets up auto-scaling

3. **Application Deployment**
   - Deploys backend, frontend, and Redis
   - Configures persistent storage
   - Sets up health checks and monitoring

## 🎯 **Accessing Your Application**

### ACI Deployment
After successful deployment, Terraform outputs will show:

```bash
# Get deployment information
terraform output

# Access URLs
Frontend: http://your-app.eastus.azurecontainer.io
Backend: http://your-app.eastus.azurecontainer.io:8080
Swagger: http://your-app.eastus.azurecontainer.io:8080/swagger
```

### AKS Deployment
For AKS deployments:

```bash
# Get cluster credentials
az aks get-credentials --resource-group $(terraform output -raw resource_group_name) --name $(terraform output -raw aks_cluster_name)

# Check application status
kubectl get pods -n lzai
kubectl get services -n lzai

# Get external IP (may take a few minutes)
kubectl get service lzai-frontend -n lzai

# Access application
curl http://EXTERNAL_IP
```

## 🔧 **Management and Operations**

### Scaling Applications

**ACI Scaling:**
```bash
# Modify terraform.tfvars
aci_backend_cpu = "2"
aci_backend_memory = "4"

# Apply changes
terraform apply
```

**AKS Scaling:**
```bash
# Scale deployments
kubectl scale deployment lzai-backend --replicas=3 -n lzai
kubectl scale deployment lzai-frontend --replicas=2 -n lzai

# Scale cluster nodes
az aks scale --resource-group $(terraform output -raw resource_group_name) --name $(terraform output -raw aks_cluster_name) --node-count 3
```

### Monitoring and Logs

```bash
# View container logs (ACI)
az container logs --resource-group $(terraform output -raw resource_group_name) --name $(terraform output -raw aci_container_group_name)

# View pod logs (AKS)
kubectl logs -l app=lzai-backend -n lzai

# Access Azure Monitor
# Visit Azure Portal → Monitor → Logs
```

### Updates and Maintenance

```bash
# Update container images
terraform apply -var="image_tag=v1.1.0"

# Update Kubernetes manifests (AKS)
kubectl apply -f k8s/

# Update infrastructure
terraform plan
terraform apply
```

## 💰 **Cost Management**

### Cost Estimates

**ACI Deployment:**
- Container Group: ~$50-100/month
- ACR Standard: ~$5/month
- Log Analytics: ~$10-20/month
- **Total: ~$65-125/month**

**AKS Deployment:**
- AKS Cluster: Free (managed service)
- VM Nodes (2x Standard_D4s_v3): ~$300/month
- ACR Standard: ~$5/month
- Log Analytics: ~$20-40/month
- **Total: ~$325-345/month**

### Cost Optimization
```bash
# Use smaller VM sizes for development
aks_node_vm_size = "Standard_B2s"

# Enable auto-scaling
aks_min_node_count = 1
aks_max_node_count = 3

# Use spot instances for non-production
# (Configure in AKS module)
```

## 🛠️ **Troubleshooting**

### Common Issues

1. **Terraform State Lock**
   ```bash
   # If state is locked
   terraform force-unlock LOCK_ID
   ```

2. **Resource Name Conflicts**
   ```bash
   # Generate new random suffix
   terraform taint random_string.suffix
   terraform apply
   ```

3. **AKS Access Issues**
   ```bash
   # Reset AKS credentials
   az aks get-credentials --resource-group RG_NAME --name CLUSTER_NAME --overwrite-existing
   ```

4. **Container Image Pull Errors**
   ```bash
   # Check ACR credentials
   terraform output acr_login_server
   az acr login --name ACR_NAME
   ```

### Debug Commands

```bash
# Terraform debugging
export TF_LOG=DEBUG
terraform apply

# Check Azure resources
az resource list --resource-group $(terraform output -raw resource_group_name) --output table

# Check Kubernetes resources
kubectl describe pod POD_NAME -n lzai
kubectl get events -n lzai --sort-by='.lastTimestamp'
```

## 🔄 **CI/CD Integration**

### GitHub Actions Example

```yaml
name: Deploy LZAI to Azure
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Terraform
      uses: hashicorp/setup-terraform@v2
      
    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
        
    - name: Terraform Init
      run: terraform init
      working-directory: terraform
      
    - name: Terraform Apply
      run: terraform apply -auto-approve
      working-directory: terraform
```

## 🧹 **Cleanup**

```bash
# Destroy all resources
terraform destroy

# Or use the script
./deploy.sh destroy
```

This comprehensive Terraform solution provides enterprise-grade deployment capabilities with proper infrastructure as code practices, monitoring, security, and scalability options for your LZAI application! 🚀
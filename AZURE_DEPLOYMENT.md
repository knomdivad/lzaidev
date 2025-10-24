# Azure Container Deployment Guide

This guide shows you how to deploy your LZAI application to Azure using either Azure Container Instances (ACI) or Azure Kubernetes Service (AKS).

## Prerequisites

1. **Azure CLI** installed and configured
2. **Docker** installed and running
3. **Azure subscription** with appropriate permissions
4. **kubectl** (for AKS deployment only)

## Option 1: Azure Container Instances (ACI) - Simplest

ACI is perfect for simple container workloads without needing to manage infrastructure.

### Step 1: Build and Push Images

```bash
# Make the script executable and run it
chmod +x ./build-and-push.sh
./build-and-push.sh
```

This script will:
- Create an Azure Container Registry (ACR)
- Build your Docker images
- Push them to ACR

### Step 2: Update Parameters

Edit `infra/aci-deployment.bicepparam`:
```bicep
using 'aci-deployment.bicep'

param containerGroupName = 'lzai-aci'
param acrName = 'YOUR_ACR_NAME'  // Replace with your ACR name
param acrResourceGroup = 'YOUR_RESOURCE_GROUP'  // Replace with your resource group
param imageTag = 'latest'
param environment = 'dev'
```

### Step 3: Deploy to ACI

```bash
# Deploy using Bicep
az deployment group create \
  --resource-group YOUR_RESOURCE_GROUP \
  --template-file infra/aci-deployment.bicep \
  --parameters infra/aci-deployment.bicepparam

# Get the deployment outputs
az deployment group show \
  --resource-group YOUR_RESOURCE_GROUP \
  --name aci-deployment \
  --query properties.outputs
```

### Step 4: Access Your Application

The deployment will output URLs:
- **Frontend**: `http://YOUR_CONTAINER_GROUP.REGION.azurecontainer.io`
- **Backend API**: `http://YOUR_CONTAINER_GROUP.REGION.azurecontainer.io:8080`
- **Swagger**: `http://YOUR_CONTAINER_GROUP.REGION.azurecontainer.io:8080/swagger`

## Option 2: Azure Kubernetes Service (AKS) - Production Ready

AKS provides orchestration, scaling, and management capabilities for production workloads.

### Step 1: Build and Push Images

```bash
# Same as ACI - build and push to ACR
./build-and-push.sh
```

### Step 2: Create AKS Cluster

Edit `infra/aks-deployment.bicepparam`:
```bicep
using 'aks-deployment.bicep'

param clusterName = 'lzai-aks'
param acrName = 'YOUR_ACR_NAME'  // Replace with your ACR name
param acrResourceGroup = 'YOUR_RESOURCE_GROUP'
param agentCount = 2
param agentVMSize = 'Standard_D2s_v3'
param environment = 'dev'
```

Deploy AKS cluster:
```bash
az deployment group create \
  --resource-group YOUR_RESOURCE_GROUP \
  --template-file infra/aks-deployment.bicep \
  --parameters infra/aks-deployment.bicepparam
```

### Step 3: Configure kubectl

```bash
# Get cluster credentials
az aks get-credentials \
  --resource-group YOUR_RESOURCE_GROUP \
  --name lzai-aks

# Verify connection
kubectl get nodes
```

### Step 4: Update Kubernetes Manifests

Update the image references in `k8s/backend.yaml` and `k8s/frontend.yaml`:
```yaml
# Replace <ACR_LOGIN_SERVER> with your actual ACR login server
image: YOUR_ACR_NAME.azurecr.io/lzai-backend:latest
image: YOUR_ACR_NAME.azurecr.io/lzai-frontend:latest
```

### Step 5: Deploy to Kubernetes

```bash
# Deploy all components
kubectl apply -f k8s/redis.yaml
kubectl apply -f k8s/backend.yaml
kubectl apply -f k8s/frontend.yaml

# Check deployment status
kubectl get pods
kubectl get services

# Get external IP (may take a few minutes)
kubectl get service lzai-frontend
```

### Step 6: Access Your Application

```bash
# Get the external IP of the frontend service
kubectl get service lzai-frontend -o jsonpath='{.status.loadBalancer.ingress[0].ip}'

# Your app will be available at: http://EXTERNAL_IP
```

## Cost Considerations

### Azure Container Instances (ACI)
- **Best for**: Development, testing, simple workloads
- **Cost**: Pay per second of execution
- **Resources**: 4 vCPU, 4GB RAM total (Backend: 1 vCPU/2GB, Frontend: 1 vCPU/1GB, Redis: 1 vCPU/1GB)
- **Estimated cost**: ~$50-100/month for continuous running

### Azure Kubernetes Service (AKS)
- **Best for**: Production, scaling, complex workloads
- **Cost**: Pay for VM nodes + small management fee
- **Resources**: 2 x Standard_D2s_v3 VMs (2 vCPU, 8GB RAM each)
- **Estimated cost**: ~$150-200/month

## Monitoring and Management

### ACI Monitoring
```bash
# View container logs
az container logs --resource-group YOUR_RESOURCE_GROUP --name lzai-aci --container-name lzai-backend

# Check container status
az container show --resource-group YOUR_RESOURCE_GROUP --name lzai-aci
```

### AKS Monitoring
```bash
# View pod logs
kubectl logs -l app=lzai-backend

# Check pod status
kubectl describe pod POD_NAME

# Scale deployments
kubectl scale deployment lzai-backend --replicas=3
```

## Cleanup

### ACI Cleanup
```bash
az container delete --resource-group YOUR_RESOURCE_GROUP --name lzai-aci --yes
```

### AKS Cleanup
```bash
az aks delete --resource-group YOUR_RESOURCE_GROUP --name lzai-aks --yes
```

## Next Steps

1. **Custom Domain**: Configure a custom domain with Azure DNS
2. **SSL/TLS**: Add HTTPS with Azure Application Gateway or cert-manager
3. **CI/CD**: Set up GitHub Actions for automated deployments
4. **Monitoring**: Add Azure Monitor and Application Insights
5. **Security**: Implement Azure Key Vault for secrets management

## Troubleshooting

### Common Issues

1. **Image Pull Errors**: Ensure ACR credentials are correctly configured
2. **Network Issues**: Check security groups and firewall rules
3. **Resource Limits**: Monitor CPU and memory usage
4. **DNS Resolution**: Verify service names and networking configuration

### Useful Commands

```bash
# Check Azure resource status
az resource list --resource-group YOUR_RESOURCE_GROUP --output table

# View deployment history
az deployment group list --resource-group YOUR_RESOURCE_GROUP

# Monitor AKS cluster
kubectl top nodes
kubectl top pods
```
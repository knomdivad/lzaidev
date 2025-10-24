using 'aks-deployment.bicep'

param clusterName = 'lzai-aks'
param acrName = 'YOUR_ACR_NAME'  // Replace with your ACR name
param acrResourceGroup = 'YOUR_RESOURCE_GROUP'  // Replace with your resource group
param agentCount = 2
param agentVMSize = 'Standard_D2s_v3'
param environment = 'dev'
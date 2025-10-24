using 'aci-deployment.bicep'

param containerGroupName = 'lzai-aci'
param acrName = 'YOUR_ACR_NAME'  // Replace with your ACR name
param acrResourceGroup = 'YOUR_RESOURCE_GROUP'  // Replace with your resource group
param imageTag = 'latest'
param environment = 'dev'
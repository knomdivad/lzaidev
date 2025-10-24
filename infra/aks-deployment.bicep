// Azure Kubernetes Service (AKS) deployment for LZAI application
// Creates AKS cluster and deploys containerized application

@description('The name of the AKS cluster')
param clusterName string = 'lzai-aks'

@description('Location for all resources')
param location string = resourceGroup().location

@description('Azure Container Registry name')
param acrName string

@description('Azure Container Registry resource group (if different from current)')
param acrResourceGroup string = resourceGroup().name

@description('Container image tag to deploy (used in Kubernetes manifests)')
param imageTag string = 'latest'

@description('Environment name (dev, staging, prod)')
@allowed(['dev', 'staging', 'prod'])
param environment string = 'dev'

@description('Number of agent nodes for the cluster')
param agentCount int = 2

@description('The size of the Virtual Machine')
param agentVMSize string = 'Standard_D2s_v3'

@description('DNS prefix for the cluster')
param dnsPrefix string = '${clusterName}-dns'

// Reference to existing ACR
resource acr 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' existing = {
  name: acrName
  scope: resourceGroup(acrResourceGroup)
}

// AKS Cluster
resource aksCluster 'Microsoft.ContainerService/managedClusters@2024-01-01' = {
  name: clusterName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: 0
        count: agentCount
        vmSize: agentVMSize
        osType: 'Linux'
        mode: 'System'
      }
    ]
    networkProfile: {
      networkPlugin: 'kubenet'
      loadBalancerSku: 'standard'
    }
    enableRBAC: true
  }
  tags: {
    Environment: environment
    Application: 'LandingZoneAI'
    DeploymentMethod: 'AKS'
  }
}

// Role assignment for AKS to pull from ACR
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (acrResourceGroup == resourceGroup().name) {
  name: guid(aksCluster.id, acr.id, 'AcrPull')
  scope: acr
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // AcrPull role
    principalId: aksCluster.properties.identityProfile.kubeletidentity.objectId
    principalType: 'ServicePrincipal'
  }
}

// Outputs
@description('Control plane FQDN')
output controlPlaneFQDN string = aksCluster.properties.fqdn

@description('Kubeconfig command')
output kubeConfigCommand string = 'az aks get-credentials --resource-group ${resourceGroup().name} --name ${clusterName}'

@description('ACR login server')
output acrLoginServer string = acr.properties.loginServer

@description('Next steps')
output nextSteps array = [
  'Run: az aks get-credentials --resource-group ${resourceGroup().name} --name ${clusterName}'
  'Apply Kubernetes manifests: kubectl apply -f k8s/'
  'Check pod status: kubectl get pods'
  'Get service URL: kubectl get service lzai-frontend'
]

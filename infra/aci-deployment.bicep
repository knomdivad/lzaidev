// Azure Container Instances (ACI) deployment for LZAI application
// Deploys containerized .NET backend, React frontend, and Redis to ACI

@description('The name of the container group')
param containerGroupName string = 'lzai-aci'

@description('Location for all resources')
param location string = resourceGroup().location

@description('Azure Container Registry name')
param acrName string

@description('Azure Container Registry resource group (if different from current)')
param acrResourceGroup string = resourceGroup().name

@description('Container image tag to deploy')
param imageTag string = 'latest'

@description('Environment name (dev, staging, prod)')
@allowed(['dev', 'staging', 'prod'])
param environment string = 'dev'

// Reference to existing ACR
resource acr 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' existing = {
  name: acrName
  scope: resourceGroup(acrResourceGroup)
}

// Container Group with all services
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: containerGroupName
  location: location
  properties: {
    sku: 'Standard'
    containers: [
      {
        name: 'lzai-backend'
        properties: {
          image: '${acr.properties.loginServer}/lzai-backend:${imageTag}'
          ports: [
            {
              port: 8080
              protocol: 'TCP'
            }
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: environment == 'prod' ? 'Production' : 'Development'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'CORS_ORIGINS'
              value: 'http://${containerGroupName}.${location}.azurecontainer.io'
            }
            {
              name: 'Redis__ConnectionString'
              value: 'localhost:6379'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 2
            }
          }
          readinessProbe: {
            httpGet: {
              path: '/health'
              port: 8080
              scheme: 'HTTP'
            }
            initialDelaySeconds: 30
            periodSeconds: 10
            timeoutSeconds: 5
            successThreshold: 1
            failureThreshold: 3
          }
          livenessProbe: {
            httpGet: {
              path: '/health'
              port: 8080
              scheme: 'HTTP'
            }
            initialDelaySeconds: 60
            periodSeconds: 30
            timeoutSeconds: 10
            successThreshold: 1
            failureThreshold: 3
          }
        }
      }
      {
        name: 'lzai-frontend'
        properties: {
          image: '${acr.properties.loginServer}/lzai-frontend:${imageTag}'
          ports: [
            {
              port: 80
              protocol: 'TCP'
            }
          ]
          environmentVariables: [
            {
              name: 'REACT_APP_API_URL'
              value: 'http://${containerGroupName}.${location}.azurecontainer.io:8080/api'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          readinessProbe: {
            httpGet: {
              path: '/'
              port: 80
              scheme: 'HTTP'
            }
            initialDelaySeconds: 30
            periodSeconds: 10
            timeoutSeconds: 5
            successThreshold: 1
            failureThreshold: 3
          }
        }
      }
      {
        name: 'redis'
        properties: {
          image: 'redis:7-alpine'
          ports: [
            {
              port: 6379
              protocol: 'TCP'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          readinessProbe: {
            exec: {
              command: ['redis-cli', 'ping']
            }
            initialDelaySeconds: 15
            periodSeconds: 10
            timeoutSeconds: 5
            successThreshold: 1
            failureThreshold: 3
          }
        }
      }
    ]
    imageRegistryCredentials: [
      {
        server: acr.properties.loginServer
        username: acr.listCredentials().username
        password: acr.listCredentials().passwords[0].value
      }
    ]
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: 80
          protocol: 'TCP'
        }
        {
          port: 8080
          protocol: 'TCP'
        }
      ]
      dnsNameLabel: containerGroupName
    }
    osType: 'Linux'
    restartPolicy: 'Always'
  }
  tags: {
    Environment: environment
    Application: 'LandingZoneAI'
    DeploymentMethod: 'ACI'
  }
}

// Outputs
@description('The FQDN of the container group')
output fqdn string = containerGroup.properties.ipAddress.fqdn

@description('The public IP address of the container group')
output ipAddress string = containerGroup.properties.ipAddress.ip

@description('Frontend URL')
output frontendUrl string = 'http://${containerGroup.properties.ipAddress.fqdn}'

@description('Backend API URL')
output backendUrl string = 'http://${containerGroup.properties.ipAddress.fqdn}:8080'

@description('Swagger URL')
output swaggerUrl string = 'http://${containerGroup.properties.ipAddress.fqdn}:8080/swagger'

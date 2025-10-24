# Resource Group
output "resource_group_name" {
  description = "Name of the main resource group"
  value       = azurerm_resource_group.main.name
}

output "resource_group_id" {
  description = "ID of the main resource group"
  value       = azurerm_resource_group.main.id
}

output "location" {
  description = "Azure region where resources are deployed"
  value       = azurerm_resource_group.main.location
}

# Networking
output "virtual_network_id" {
  description = "ID of the virtual network"
  value       = module.networking.virtual_network_id
}

output "virtual_network_name" {
  description = "Name of the virtual network"
  value       = module.networking.virtual_network_name
}

output "subnet_ids" {
  description = "Map of subnet names to IDs"
  value       = module.networking.subnet_ids
}

output "public_subnet_id" {
  description = "ID of the public subnet"
  value       = module.networking.public_subnet_id
}

output "private_subnet_id" {
  description = "ID of the private subnet"
  value       = module.networking.private_subnet_id
}

# Security
output "key_vault_id" {
  description = "ID of the Key Vault"
  value       = module.security.key_vault_id
}

output "key_vault_name" {
  description = "Name of the Key Vault"
  value       = module.security.key_vault_name
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = module.security.key_vault_uri
}

# Storage
output "storage_account_id" {
  description = "ID of the storage account"
  value       = module.storage.storage_account_id
}

output "storage_account_name" {
  description = "Name of the storage account"
  value       = module.storage.storage_account_name
}

output "storage_account_primary_blob_endpoint" {
  description = "Primary blob endpoint of the storage account"
  value       = module.storage.storage_account_primary_blob_endpoint
}

# Container Registry
output "container_registry_id" {
  description = "ID of the container registry"
  value       = module.container_registry.registry_id
}

output "container_registry_name" {
  description = "Name of the container registry"
  value       = module.container_registry.registry_name
}

output "container_registry_login_server" {
  description = "Login server of the container registry"
  value       = module.container_registry.registry_login_server
}

# AKS
output "aks_cluster_id" {
  description = "ID of the AKS cluster"
  value       = module.aks.cluster_id
}

output "aks_cluster_name" {
  description = "Name of the AKS cluster"
  value       = module.aks.cluster_name
}

output "aks_cluster_fqdn" {
  description = "FQDN of the AKS cluster"
  value       = module.aks.cluster_fqdn
}

output "aks_cluster_endpoint" {
  description = "Endpoint of the AKS cluster"
  value       = module.aks.cluster_endpoint
  sensitive   = true
}

output "aks_kubelet_identity" {
  description = "Kubelet identity of the AKS cluster"
  value       = module.aks.kubelet_identity
}

# Monitoring
output "log_analytics_workspace_id" {
  description = "ID of the Log Analytics workspace"
  value       = module.monitoring.log_analytics_workspace_id
}

output "log_analytics_workspace_name" {
  description = "Name of the Log Analytics workspace"
  value       = module.monitoring.log_analytics_workspace_name
}

output "application_insights_id" {
  description = "ID of Application Insights"
  value       = module.monitoring.app_insights_id
}

output "application_insights_instrumentation_key" {
  description = "Instrumentation key for Application Insights"
  value       = module.monitoring.app_insights_instrumentation_key
  sensitive   = true
}

output "application_insights_connection_string" {
  description = "Connection string for Application Insights"
  value       = module.monitoring.app_insights_connection_string
  sensitive   = true
}

# AI Services
output "ml_workspace_id" {
  description = "ID of the Machine Learning workspace"
  value       = module.ai_services.ml_workspace_id
}

output "ml_workspace_name" {
  description = "Name of the Machine Learning workspace"
  value       = module.ai_services.ml_workspace_name
}

output "cognitive_services_id" {
  description = "ID of Cognitive Services"
  value       = module.ai_services.cognitive_services_id
}

output "cognitive_services_endpoint" {
  description = "Endpoint of Cognitive Services"
  value       = module.ai_services.cognitive_services_endpoint
}

output "openai_service_id" {
  description = "ID of OpenAI Service"
  value       = module.ai_services.openai_service_id
}

output "openai_service_endpoint" {
  description = "Endpoint of OpenAI Service"
  value       = module.ai_services.openai_service_endpoint
}

# Connection Information
output "connection_info" {
  description = "Connection information for applications"
  value = {
    resource_group_name = azurerm_resource_group.main.name
    location           = azurerm_resource_group.main.location
    key_vault_name     = module.security.key_vault_name
    storage_account_name = module.storage.storage_account_name
    container_registry_name = module.container_registry.registry_name
    aks_cluster_name   = module.aks.cluster_name
    ml_workspace_name  = module.ai_services.ml_workspace_name
  }
}

# Resource Naming
output "resource_naming" {
  description = "Resource naming convention used"
  value = {
    resource_group      = local.naming.resource_group
    virtual_network     = local.naming.virtual_network
    key_vault          = local.naming.key_vault
    storage_account    = local.naming.storage_account
    container_registry = local.naming.container_registry
    aks_cluster        = local.naming.aks_cluster
    log_analytics      = local.naming.log_analytics
    app_insights       = local.naming.app_insights
    ml_workspace       = local.naming.ml_workspace
    cognitive_services = local.naming.cognitive_services
  }
}

# LZAI Deployment Outputs

# ACR Outputs for LZAI
output "acr_login_server" {
  description = "Login server of the Azure Container Registry"
  value       = module.container_registry.login_server
}

output "acr_admin_username" {
  description = "Admin username for ACR"
  value       = module.container_registry.admin_username
  sensitive   = true
}

output "acr_admin_password" {
  description = "Admin password for ACR"
  value       = module.container_registry.admin_password
  sensitive   = true
}

# LZAI ACI Outputs (conditional)
output "aci_container_group_id" {
  description = "ID of the ACI container group"
  value       = var.deployment_type == "aci" && length(module.lzai_aci) > 0 ? module.lzai_aci[0].container_group_id : null
}

output "aci_fqdn" {
  description = "FQDN of the ACI container group"
  value       = var.deployment_type == "aci" && length(module.lzai_aci) > 0 ? module.lzai_aci[0].fqdn : null
}

output "aci_frontend_url" {
  description = "Frontend URL for ACI deployment"
  value       = var.deployment_type == "aci" && length(module.lzai_aci) > 0 ? module.lzai_aci[0].frontend_url : null
}

output "aci_backend_url" {
  description = "Backend URL for ACI deployment"
  value       = var.deployment_type == "aci" && length(module.lzai_aci) > 0 ? module.lzai_aci[0].backend_url : null
}

output "aci_swagger_url" {
  description = "Swagger URL for ACI deployment"
  value       = var.deployment_type == "aci" && length(module.lzai_aci) > 0 ? module.lzai_aci[0].swagger_url : null
}

# AKS Outputs for LZAI
output "aks_cluster_name" {
  description = "Name of the AKS cluster"
  value       = module.aks.cluster_name
}

output "aks_kube_config" {
  description = "Kubernetes configuration for AKS"
  value       = module.aks.kube_config
  sensitive   = true
}

# Deployment Information
output "deployment_type" {
  description = "Type of deployment (aci or aks)"
  value       = var.deployment_type
}

output "environment" {
  description = "Environment name"
  value       = var.environment
}

output "project_name" {
  description = "Project name"
  value       = var.project_name
}
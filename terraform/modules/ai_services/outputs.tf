# Outputs for AI Services Module

# Machine Learning Workspace
output "ml_workspace_id" {
  description = "The ID of the Azure Machine Learning workspace"
  value       = azurerm_machine_learning_workspace.main.id
}

output "ml_workspace_name" {
  description = "The name of the Azure Machine Learning workspace"
  value       = azurerm_machine_learning_workspace.main.name
}

output "ml_workspace_discovery_url" {
  description = "The discovery URL of the Azure Machine Learning workspace"
  value       = azurerm_machine_learning_workspace.main.discovery_url
}

# Cognitive Services
output "cognitive_services_id" {
  description = "The ID of the Cognitive Services account"
  value       = azurerm_cognitive_account.main.id
}

output "cognitive_services_endpoint" {
  description = "The endpoint of the Cognitive Services account"
  value       = azurerm_cognitive_account.main.endpoint
}

output "cognitive_services_primary_access_key" {
  description = "The primary access key for the Cognitive Services account"
  value       = azurerm_cognitive_account.main.primary_access_key
  sensitive   = true
}

# OpenAI Services
output "openai_id" {
  description = "The ID of the Azure OpenAI account"
  value       = azurerm_cognitive_account.openai.id
}

output "openai_endpoint" {
  description = "The endpoint of the Azure OpenAI account"
  value       = azurerm_cognitive_account.openai.endpoint
}

output "openai_primary_access_key" {
  description = "The primary access key for the Azure OpenAI account"
  value       = azurerm_cognitive_account.openai.primary_access_key
  sensitive   = true
}

output "openai_model_deployments" {
  description = "Information about deployed OpenAI models"
  value = {
    for k, v in azurerm_cognitive_deployment.openai_models : k => {
      id       = v.id
      name     = v.name
      endpoint = "${azurerm_cognitive_account.openai.endpoint}openai/deployments/${v.name}"
    }
  }
}

# Azure AI Search
output "search_service_id" {
  description = "The ID of the Azure AI Search service"
  value       = azurerm_search_service.main.id
}

output "search_service_name" {
  description = "The name of the Azure AI Search service"
  value       = azurerm_search_service.main.name
}

output "search_service_url" {
  description = "The URL of the Azure AI Search service"
  value       = "https://${azurerm_search_service.main.name}.search.windows.net"
}

output "search_service_primary_key" {
  description = "The primary admin key for the Azure AI Search service"
  value       = azurerm_search_service.main.primary_key
  sensitive   = true
}

output "search_service_query_keys" {
  description = "The query keys for the Azure AI Search service"
  value       = azurerm_search_service.main.query_keys
  sensitive   = true
}

# Private Endpoints
output "private_endpoints" {
  description = "Information about private endpoints"
  value = var.enable_private_endpoint ? {
    ml_workspace = {
      id                = azurerm_private_endpoint.ml_workspace[0].id
      private_ip_address = azurerm_private_endpoint.ml_workspace[0].private_service_connection[0].private_ip_address
    }
    cognitive_services = {
      id                = azurerm_private_endpoint.cognitive_services[0].id
      private_ip_address = azurerm_private_endpoint.cognitive_services[0].private_service_connection[0].private_ip_address
    }
    openai = {
      id                = azurerm_private_endpoint.openai[0].id
      private_ip_address = azurerm_private_endpoint.openai[0].private_service_connection[0].private_ip_address
    }
  } : {}
}
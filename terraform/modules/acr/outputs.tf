# Azure Container Registry Module Outputs

output "registry_id" {
  description = "ID of the Azure Container Registry"
  value       = azurerm_container_registry.main.id
}

output "registry_name" {
  description = "Name of the Azure Container Registry"
  value       = azurerm_container_registry.main.name
}

output "login_server" {
  description = "Login server URL of the Azure Container Registry"
  value       = azurerm_container_registry.main.login_server
}

output "admin_username" {
  description = "Admin username for the Azure Container Registry"
  value       = azurerm_container_registry.main.admin_username
  sensitive   = true
}

output "admin_password" {
  description = "Admin password for the Azure Container Registry"
  value       = azurerm_container_registry.main.admin_password
  sensitive   = true
}

output "identity_principal_id" {
  description = "Principal ID of the system assigned identity"
  value       = azurerm_container_registry.main.identity[0].principal_id
}

output "private_endpoint_ip" {
  description = "Private IP address of the private endpoint"
  value       = var.enable_private_endpoint ? azurerm_private_endpoint.acr[0].private_service_connection[0].private_ip_address : null
}
output "registry_id" {
  description = "ID of the container registry"
  value       = azurerm_container_registry.main.id
}

output "registry_name" {
  description = "Name of the container registry"
  value       = azurerm_container_registry.main.name
}

output "registry_login_server" {
  description = "Login server of the container registry"
  value       = azurerm_container_registry.main.login_server
}

output "admin_username" {
  description = "Admin username for the registry"
  value       = var.admin_enabled ? azurerm_container_registry.main.admin_username : null
}

output "admin_password" {
  description = "Admin password for the registry"
  value       = var.admin_enabled ? azurerm_container_registry.main.admin_password : null
  sensitive   = true
}

output "private_endpoint_id" {
  description = "ID of the container registry private endpoint"
  value       = var.enable_private_endpoint ? azurerm_private_endpoint.registry[0].id : null
}
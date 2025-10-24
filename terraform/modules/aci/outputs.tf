# Azure Container Instances Module Outputs

output "container_group_id" {
  description = "ID of the container group"
  value       = azurerm_container_group.lzai.id
}

output "container_group_name" {
  description = "Name of the container group"
  value       = azurerm_container_group.lzai.name
}

output "fqdn" {
  description = "Fully qualified domain name of the container group"
  value       = azurerm_container_group.lzai.fqdn
}

output "ip_address" {
  description = "Public IP address of the container group"
  value       = azurerm_container_group.lzai.ip_address
}

output "frontend_url" {
  description = "URL to access the frontend application"
  value       = "http://${azurerm_container_group.lzai.fqdn}"
}

output "backend_url" {
  description = "URL to access the backend API"
  value       = "http://${azurerm_container_group.lzai.fqdn}:8080"
}

output "swagger_url" {
  description = "URL to access the Swagger documentation"
  value       = "http://${azurerm_container_group.lzai.fqdn}:8080/swagger"
}
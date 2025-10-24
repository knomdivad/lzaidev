output "key_vault_id" {
  description = "ID of the Key Vault"
  value       = azurerm_key_vault.main.id
}

output "key_vault_name" {
  description = "Name of the Key Vault"
  value       = azurerm_key_vault.main.name
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = azurerm_key_vault.main.vault_uri
}

output "encryption_key_id" {
  description = "ID of the encryption key"
  value       = azurerm_key_vault_key.encryption_key.id
}

output "encryption_key_version_id" {
  description = "Version ID of the encryption key"
  value       = azurerm_key_vault_key.encryption_key.version_id
}

output "database_password_secret_id" {
  description = "ID of the database password secret"
  value       = azurerm_key_vault_secret.database_password.id
  sensitive   = true
}

output "storage_key_secret_id" {
  description = "ID of the storage key secret"
  value       = azurerm_key_vault_secret.storage_key.id
  sensitive   = true
}

output "private_endpoint_id" {
  description = "ID of the Key Vault private endpoint"
  value       = var.enable_private_endpoint ? azurerm_private_endpoint.key_vault[0].id : null
}
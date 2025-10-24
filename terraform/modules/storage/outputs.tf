output "storage_account_id" {
  description = "ID of the storage account"
  value       = azurerm_storage_account.main.id
}

output "storage_account_name" {
  description = "Name of the storage account"
  value       = azurerm_storage_account.main.name
}

output "storage_account_primary_blob_endpoint" {
  description = "Primary blob endpoint of the storage account"
  value       = azurerm_storage_account.main.primary_blob_endpoint
}

output "storage_account_primary_dfs_endpoint" {
  description = "Primary Data Lake Storage endpoint"
  value       = azurerm_storage_account.main.primary_dfs_endpoint
}

output "storage_account_primary_access_key" {
  description = "Primary access key of the storage account"
  value       = azurerm_storage_account.main.primary_access_key
  sensitive   = true
}

output "container_names" {
  description = "Names of created containers"
  value       = concat(
    [for k, v in azurerm_storage_container.containers : v.name],
    [for k, v in azurerm_storage_container.default_containers : v.name]
  )
}

output "private_endpoint_blob_id" {
  description = "ID of the blob private endpoint"
  value       = var.enable_private_endpoint ? azurerm_private_endpoint.storage_blob[0].id : null
}

output "private_endpoint_dfs_id" {
  description = "ID of the DFS private endpoint"
  value       = var.enable_private_endpoint ? azurerm_private_endpoint.storage_dfs[0].id : null
}
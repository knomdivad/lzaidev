# Storage Module - Azure Storage Account for AI/ML workloads

# Storage Account
resource "azurerm_storage_account" "main" {
  name                     = var.storage_account_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.account_tier
  account_replication_type = var.replication_type
  account_kind             = "StorageV2"
  
  # Security settings
  min_tls_version                = "TLS1_2"
  allow_nested_items_to_be_public = false
  shared_access_key_enabled      = true
  public_network_access_enabled  = var.enable_private_endpoint ? false : true
  
  # Advanced threat protection
  enable_https_traffic_only = true
  
  # Data Lake Gen2
  is_hns_enabled = true

  # Network rules
  network_rules {
    default_action = var.enable_private_endpoint ? "Deny" : "Allow"
    bypass         = ["AzureServices"]
  }

  # Blob properties
  blob_properties {
    # Enable versioning
    versioning_enabled = true
    
    # Enable change feed
    change_feed_enabled = true
    
    # Point-in-time restore
    restore_policy {
      days = 7
    }
    
    # Container delete retention
    container_delete_retention_policy {
      days = 7
    }
    
    # Blob delete retention
    delete_retention_policy {
      days = 7
    }
  }

  tags = var.tags
}

# Storage Containers for AI/ML workloads
resource "azurerm_storage_container" "containers" {
  for_each = var.containers

  name                  = each.key
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = each.value.access_type
}

# Default containers for AI/ML
locals {
  default_containers = {
    "datasets" = {
      access_type = "private"
    }
    "models" = {
      access_type = "private"
    }
    "experiments" = {
      access_type = "private"
    }
    "artifacts" = {
      access_type = "private"
    }
    "logs" = {
      access_type = "private"
    }
  }
}

resource "azurerm_storage_container" "default_containers" {
  for_each = local.default_containers

  name                  = each.key
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = each.value.access_type
}

# Private endpoint for Storage Account (if enabled)
resource "azurerm_private_endpoint" "storage_blob" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.storage_account_name}-blob-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_subnet_id

  private_service_connection {
    name                           = "${var.storage_account_name}-blob-psc"
    private_connection_resource_id = azurerm_storage_account.main.id
    is_manual_connection           = false
    subresource_names              = ["blob"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.storage_blob[0].id]
  }

  tags = var.tags
}

# Private endpoint for Data Lake (if enabled)
resource "azurerm_private_endpoint" "storage_dfs" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.storage_account_name}-dfs-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_subnet_id

  private_service_connection {
    name                           = "${var.storage_account_name}-dfs-psc"
    private_connection_resource_id = azurerm_storage_account.main.id
    is_manual_connection           = false
    subresource_names              = ["dfs"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.storage_dfs[0].id]
  }

  tags = var.tags
}

# Private DNS Zone for Storage Blob
resource "azurerm_private_dns_zone" "storage_blob" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.blob.core.windows.net"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Private DNS Zone for Storage DFS
resource "azurerm_private_dns_zone" "storage_dfs" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.dfs.core.windows.net"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Link private DNS zones to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "storage_blob" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.storage_account_name}-blob-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.storage_blob[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "storage_dfs" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.storage_account_name}-dfs-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.storage_dfs[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}

# Storage Account role assignments for managed identities
resource "azurerm_role_assignment" "storage_blob_data_contributor" {
  count                = var.aks_kubelet_identity_principal_id != null ? 1 : 0
  scope                = azurerm_storage_account.main.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = var.aks_kubelet_identity_principal_id
}
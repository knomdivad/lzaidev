# Security Module - Azure Key Vault

# Get current user/service principal for Key Vault access
data "azurerm_client_config" "current" {}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = var.key_vault_name
  location            = var.location
  resource_group_name = var.resource_group_name
  tenant_id           = var.tenant_id
  sku_name            = "standard"
  
  # Security settings
  enabled_for_disk_encryption     = true
  enabled_for_deployment          = true
  enabled_for_template_deployment = true
  enable_rbac_authorization       = true
  purge_protection_enabled        = true
  soft_delete_retention_days      = var.soft_delete_retention_days

  # Network access rules
  network_acls {
    default_action = "Deny"
    bypass         = "AzureServices"
  }

  tags = var.tags
}

# Key Vault access policy for current user/service principal
resource "azurerm_role_assignment" "key_vault_admin" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = data.azurerm_client_config.current.object_id
}

# Key Vault access policy for AKS managed identity (will be added later)
resource "azurerm_role_assignment" "key_vault_secrets_user" {
  count                = var.aks_kubelet_identity_object_id != null ? 1 : 0
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.aks_kubelet_identity_object_id
}

# Generate random password for database
resource "random_password" "database_password" {
  length  = 32
  special = true
}

# Store database password in Key Vault
resource "azurerm_key_vault_secret" "database_password" {
  name         = "database-password"
  value        = random_password.database_password.result
  key_vault_id = azurerm_key_vault.main.id
  
  depends_on = [azurerm_role_assignment.key_vault_admin]
}

# Generate random storage account key (for demonstration)
resource "random_password" "storage_key" {
  length  = 64
  special = false
  upper   = true
  lower   = true
  numeric = true
}

# Store storage key in Key Vault
resource "azurerm_key_vault_secret" "storage_key" {
  name         = "storage-key"
  value        = random_password.storage_key.result
  key_vault_id = azurerm_key_vault.main.id
  
  depends_on = [azurerm_role_assignment.key_vault_admin]
}

# Key for encryption
resource "azurerm_key_vault_key" "encryption_key" {
  name         = "encryption-key"
  key_vault_id = azurerm_key_vault.main.id
  key_type     = "RSA"
  key_size     = 2048

  key_opts = [
    "decrypt",
    "encrypt",
    "sign",
    "unwrapKey",
    "verify",
    "wrapKey",
  ]

  depends_on = [azurerm_role_assignment.key_vault_admin]
}

# Private endpoint for Key Vault (if enabled)
resource "azurerm_private_endpoint" "key_vault" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.key_vault_name}-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_subnet_id

  private_service_connection {
    name                           = "${var.key_vault_name}-psc"
    private_connection_resource_id = azurerm_key_vault.main.id
    is_manual_connection           = false
    subresource_names              = ["vault"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.key_vault[0].id]
  }

  tags = var.tags
}

# Private DNS Zone for Key Vault
resource "azurerm_private_dns_zone" "key_vault" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.vaultcore.azure.net"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Link private DNS zone to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "key_vault" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.key_vault_name}-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.key_vault[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}
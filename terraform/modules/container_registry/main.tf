# Container Registry Module - Azure Container Registry

# Container Registry
resource "azurerm_container_registry" "main" {
  name                = var.registry_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = var.admin_enabled
  
  # Security settings
  public_network_access_enabled = var.enable_private_endpoint ? false : true
  network_rule_bypass_option    = "AzureServices"

  # Enable quarantine policy for security scanning
  quarantine_policy_enabled = true
  
  # Enable retention policy
  retention_policy {
    days    = var.retention_days
    enabled = true
  }

  # Trust policy for content signing
  trust_policy {
    enabled = true
  }

  # Geographic replication (for premium SKU)
  dynamic "georeplications" {
    for_each = var.sku == "Premium" ? var.georeplications : []
    content {
      location                = georeplications.value.location
      zone_redundancy_enabled = georeplications.value.zone_redundancy_enabled
      tags                    = var.tags
    }
  }

  # Network rule set
  dynamic "network_rule_set" {
    for_each = var.enable_private_endpoint ? [1] : []
    content {
      default_action = "Deny"

      # Allow Azure services
      ip_rule {
        action   = "Allow"
        ip_range = "0.0.0.0/0"
      }
    }
  }

  tags = var.tags
}

# Private endpoint for Container Registry (if enabled)
resource "azurerm_private_endpoint" "registry" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.registry_name}-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_subnet_id

  private_service_connection {
    name                           = "${var.registry_name}-psc"
    private_connection_resource_id = azurerm_container_registry.main.id
    is_manual_connection           = false
    subresource_names              = ["registry"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.registry[0].id]
  }

  tags = var.tags
}

# Private DNS Zone for Container Registry
resource "azurerm_private_dns_zone" "registry" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.azurecr.io"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Link private DNS zone to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "registry" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.registry_name}-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.registry[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}

# Role assignment for AKS to pull images
resource "azurerm_role_assignment" "aks_acr_pull" {
  count                = var.aks_kubelet_identity_principal_id != null ? 1 : 0
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPull"
  principal_id         = var.aks_kubelet_identity_principal_id
}

# Container Registry webhooks for CI/CD
resource "azurerm_container_registry_webhook" "ci_webhook" {
  count               = length(var.webhooks)
  name                = var.webhooks[count.index].name
  resource_group_name = var.resource_group_name
  registry_name       = azurerm_container_registry.main.name
  location            = var.location
  
  service_uri = var.webhooks[count.index].service_uri
  status      = var.webhooks[count.index].status
  scope       = var.webhooks[count.index].scope
  actions     = var.webhooks[count.index].actions
  
  custom_headers = var.webhooks[count.index].custom_headers

  tags = var.tags
}
# Azure Container Registry Module

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

# Azure Container Registry
resource "azurerm_container_registry" "main" {
  name                = var.registry_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = var.admin_enabled

  # Network access rules
  public_network_access_enabled = var.public_network_access_enabled
  
  # Anonymous pull is disabled by default for security
  anonymous_pull_enabled = false

  # Zone redundancy for higher availability
  zone_redundancy_enabled = var.zone_redundancy_enabled

  # Encryption
  encryption {
    enabled = var.encryption_enabled
  }

  # Retention policy for untagged manifests
  retention_policy {
    days    = var.retention_days
    enabled = var.retention_enabled
  }

  # Trust policy
  trust_policy {
    enabled = var.trust_policy_enabled
  }

  # Quarantine policy
  quarantine_policy {
    enabled = var.quarantine_policy_enabled
  }

  tags = var.tags
}

# Diagnostic settings for monitoring
resource "azurerm_monitor_diagnostic_setting" "acr" {
  count                      = var.log_analytics_workspace_id != null ? 1 : 0
  name                       = "${var.registry_name}-diagnostics"
  target_resource_id         = azurerm_container_registry.main.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category = "ContainerRegistryRepositoryEvents"
  }

  enabled_log {
    category = "ContainerRegistryLoginEvents"
  }

  metric {
    category = "AllMetrics"
    enabled  = true
  }
}

# Private endpoint for secure access (optional)
resource "azurerm_private_endpoint" "acr" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.registry_name}-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_endpoint_subnet_id

  private_service_connection {
    name                           = "${var.registry_name}-psc"
    private_connection_resource_id = azurerm_container_registry.main.id
    subresource_names              = ["registry"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = var.private_dns_zone_ids
  }

  tags = var.tags
}

# Build task for automated image building (optional)
resource "azurerm_container_registry_task" "build_task" {
  count                    = var.enable_build_task ? 1 : 0
  name                     = "lzai-build-task"
  container_registry_id    = azurerm_container_registry.main.id
  platform_os              = "Linux"
  platform_architecture    = "amd64"

  docker_step {
    dockerfile_path      = "Dockerfile"
    context_path         = var.build_context_path
    context_access_token = var.github_token
    image_names          = ["lzai-backend:{{.Run.ID}}", "lzai-backend:latest"]
  }

  source_trigger {
    name           = "github-trigger"
    events         = ["commit"]
    repository_url = var.repository_url
    source_type    = "Github"
    branch         = "main"
  }

  tags = var.tags
}
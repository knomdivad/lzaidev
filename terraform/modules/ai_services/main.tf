# AI Services Module - Azure AI and ML Services

# Azure Machine Learning Workspace
resource "azurerm_machine_learning_workspace" "main" {
  name                    = var.ml_workspace_name
  location                = var.location
  resource_group_name     = var.resource_group_name
  application_insights_id = var.app_insights_id
  key_vault_id           = var.key_vault_id
  storage_account_id     = var.storage_account_id
  
  # Identity
  identity {
    type = "SystemAssigned"
  }

  # Network settings
  public_network_access_enabled = var.enable_private_endpoint ? false : true
  
  tags = var.tags
}

# Cognitive Services Multi-Service Account
resource "azurerm_cognitive_account" "main" {
  name                = var.cognitive_services_name
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = "CognitiveServices"
  sku_name            = var.cognitive_services_sku
  
  # Network settings
  public_network_access_enabled = var.enable_private_endpoint ? false : true
  
  tags = var.tags
}

# Azure OpenAI Service
resource "azurerm_cognitive_account" "openai" {
  name                = "${var.cognitive_services_name}-openai"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = "OpenAI"
  sku_name            = "S0"
  
  # Network settings
  public_network_access_enabled = var.enable_private_endpoint ? false : true
  
  tags = var.tags
}

# OpenAI Model Deployments
resource "azurerm_cognitive_deployment" "openai_models" {
  for_each = var.openai_models

  name                 = each.value.name
  cognitive_account_id = azurerm_cognitive_account.openai.id
  
  model {
    format  = "OpenAI"
    name    = each.value.model_name
    version = each.value.model_version
  }
  
  sku {
    name     = "Standard"
    capacity = each.value.capacity
  }
}

# Azure AI Search Service
resource "azurerm_search_service" "main" {
  name                          = var.search_service_name
  resource_group_name           = var.resource_group_name
  location                      = var.location
  sku                          = var.search_service_sku
  public_network_access_enabled = var.enable_private_endpoint ? false : true
  
  tags = var.tags
}

# Private endpoints for AI services (if enabled)
resource "azurerm_private_endpoint" "ml_workspace" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.ml_workspace_name}-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.subnet_id

  private_service_connection {
    name                           = "${var.ml_workspace_name}-psc"
    private_connection_resource_id = azurerm_machine_learning_workspace.main.id
    is_manual_connection           = false
    subresource_names              = ["amlworkspace"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.ml_workspace[0].id]
  }

  tags = var.tags
}

resource "azurerm_private_endpoint" "cognitive_services" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.cognitive_services_name}-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.subnet_id

  private_service_connection {
    name                           = "${var.cognitive_services_name}-psc"
    private_connection_resource_id = azurerm_cognitive_account.main.id
    is_manual_connection           = false
    subresource_names              = ["account"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.cognitive_services[0].id]
  }

  tags = var.tags
}

resource "azurerm_private_endpoint" "openai" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "${var.cognitive_services_name}-openai-pe"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.subnet_id

  private_service_connection {
    name                           = "${var.cognitive_services_name}-openai-psc"
    private_connection_resource_id = azurerm_cognitive_account.openai.id
    is_manual_connection           = false
    subresource_names              = ["account"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.cognitive_services[0].id]
  }

  tags = var.tags
}

# Private DNS Zones
resource "azurerm_private_dns_zone" "ml_workspace" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.api.azureml.ms"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

resource "azurerm_private_dns_zone" "cognitive_services" {
  count               = var.enable_private_endpoint ? 1 : 0
  name                = "privatelink.cognitiveservices.azure.com"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Link private DNS zones to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "ml_workspace" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.ml_workspace_name}-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.ml_workspace[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "cognitive_services" {
  count                 = var.enable_private_endpoint ? 1 : 0
  name                  = "${var.cognitive_services_name}-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.cognitive_services[0].name
  virtual_network_id    = var.virtual_network_id
  registration_enabled  = false
  tags                  = var.tags
}
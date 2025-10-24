# Monitoring Module - Log Analytics and Application Insights

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "main" {
  name                = var.log_analytics_name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = var.log_analytics_sku
  retention_in_days   = var.retention_days
  daily_quota_gb      = var.daily_quota_gb

  tags = var.tags
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = var.app_insights_name
  location            = var.location
  resource_group_name = var.resource_group_name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  retention_in_days   = var.retention_days

  tags = var.tags
}

# Log Analytics Solutions
resource "azurerm_log_analytics_solution" "container_insights" {
  solution_name         = "ContainerInsights"
  location              = var.location
  resource_group_name   = var.resource_group_name
  workspace_resource_id = azurerm_log_analytics_workspace.main.id
  workspace_name        = azurerm_log_analytics_workspace.main.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }

  tags = var.tags
}

resource "azurerm_log_analytics_solution" "security_insights" {
  solution_name         = "Security"
  location              = var.location
  resource_group_name   = var.resource_group_name
  workspace_resource_id = azurerm_log_analytics_workspace.main.id
  workspace_name        = azurerm_log_analytics_workspace.main.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/Security"
  }

  tags = var.tags
}

# Data Collection Rules for Azure Monitor
resource "azurerm_monitor_data_collection_rule" "main" {
  name                = "${var.log_analytics_name}-dcr"
  resource_group_name = var.resource_group_name
  location            = var.location
  description         = "Data collection rule for AI Landing Zone"

  destinations {
    log_analytics {
      workspace_resource_id = azurerm_log_analytics_workspace.main.id
      name                  = "destination-log"
    }
  }

  data_flow {
    streams      = ["Microsoft-Event"]
    destinations = ["destination-log"]
  }

  data_sources {
    windows_event_log {
      streams = ["Microsoft-Event"]
      x_path_queries = [
        "Application!*[System[(Level=1 or Level=2 or Level=3 or Level=4 or Level=0)]]",
        "Security!*[System[(band(Keywords,13510798882111488))]]",
        "System!*[System[(Level=1 or Level=2 or Level=3 or Level=4 or Level=0)]]"
      ]
      name = "eventLogsDataSource"
    }
  }

  tags = var.tags
}

# Action Groups for alerting
resource "azurerm_monitor_action_group" "main" {
  name                = "${var.log_analytics_name}-action-group"
  resource_group_name = var.resource_group_name
  short_name          = "ailzalerts"

  dynamic "email_receiver" {
    for_each = var.alert_email_addresses
    content {
      name          = "email-${email_receiver.key}"
      email_address = email_receiver.value
    }
  }

  tags = var.tags
}

# Diagnostic Settings for Key Resources (will be configured per resource)
# These will be created in the respective modules
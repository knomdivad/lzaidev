# Azure Container Instances Module for LZAI Application

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

# Container Group with all LZAI services
resource "azurerm_container_group" "lzai" {
  name                = var.container_group_name
  location            = var.location
  resource_group_name = var.resource_group_name
  ip_address_type     = "Public"
  dns_name_label      = var.dns_name_label
  os_type             = "Linux"
  restart_policy      = "Always"

  # Backend container
  container {
    name   = "lzai-backend"
    image  = "${var.acr_login_server}/lzai-backend:${var.image_tag}"
    cpu    = var.backend_cpu
    memory = var.backend_memory

    ports {
      port     = 8080
      protocol = "TCP"
    }

    environment_variables = {
      ASPNETCORE_ENVIRONMENT = var.environment == "prod" ? "Production" : "Development"
      ASPNETCORE_URLS        = "http://+:8080"
      CORS_ORIGINS          = "http://${var.dns_name_label}.${var.location}.azurecontainer.io"
      Redis__ConnectionString = "localhost:6379"
    }

    readiness_probe {
      http_get {
        path   = "/health"
        port   = 8080
        scheme = "Http"
      }
      initial_delay_seconds = 30
      period_seconds       = 10
      timeout_seconds      = 5
      success_threshold    = 1
      failure_threshold    = 3
    }

    liveness_probe {
      http_get {
        path   = "/health"
        port   = 8080
        scheme = "Http"
      }
      initial_delay_seconds = 60
      period_seconds       = 30
      timeout_seconds      = 10
      success_threshold    = 1
      failure_threshold    = 3
    }
  }

  # Frontend container
  container {
    name   = "lzai-frontend"
    image  = "${var.acr_login_server}/lzai-frontend:${var.image_tag}"
    cpu    = var.frontend_cpu
    memory = var.frontend_memory

    ports {
      port     = 80
      protocol = "TCP"
    }

    environment_variables = {
      REACT_APP_API_URL = "http://${var.dns_name_label}.${var.location}.azurecontainer.io:8080/api"
    }

    readiness_probe {
      http_get {
        path   = "/"
        port   = 80
        scheme = "Http"
      }
      initial_delay_seconds = 30
      period_seconds       = 10
      timeout_seconds      = 5
      success_threshold    = 1
      failure_threshold    = 3
    }

    liveness_probe {
      http_get {
        path   = "/"
        port   = 80
        scheme = "Http"
      }
      initial_delay_seconds = 60
      period_seconds       = 30
      timeout_seconds      = 10
      success_threshold    = 1
      failure_threshold    = 3
    }
  }

  # Redis container
  container {
    name   = "redis"
    image  = "redis:7-alpine"
    cpu    = var.redis_cpu
    memory = var.redis_memory

    ports {
      port     = 6379
      protocol = "TCP"
    }

    readiness_probe {
      exec = ["redis-cli", "ping"]
      initial_delay_seconds = 15
      period_seconds       = 10
      timeout_seconds      = 5
      success_threshold    = 1
      failure_threshold    = 3
    }

    liveness_probe {
      exec = ["redis-cli", "ping"]
      initial_delay_seconds = 30
      period_seconds       = 30
      timeout_seconds      = 10
      success_threshold    = 1
      failure_threshold    = 3
    }
  }

  # ACR credentials for private registry access
  image_registry_credential {
    server   = var.acr_login_server
    username = var.acr_admin_username
    password = var.acr_admin_password
  }

  # Exposed ports
  exposed_port {
    port     = 80
    protocol = "TCP"
  }

  exposed_port {
    port     = 8080
    protocol = "TCP"
  }

  tags = var.tags
}

# Diagnostic settings for monitoring
resource "azurerm_monitor_diagnostic_setting" "aci" {
  count                      = var.log_analytics_workspace_id != null ? 1 : 0
  name                       = "${var.container_group_name}-diagnostics"
  target_resource_id         = azurerm_container_group.lzai.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category = "ContainerInstanceLog"
  }

  metric {
    category = "AllMetrics"
  }
}
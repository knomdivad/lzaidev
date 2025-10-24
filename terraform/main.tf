terraform {
  required_version = ">= 1.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80"
    }
    azapi = {
      source  = "azure/azapi"
      version = "~> 1.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.4"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.23"
    }
  }

  backend "azurerm" {
    # Configuration will be provided via environment variables or backend config file
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

provider "azapi" {}

provider "random" {}

# Data sources
data "azurerm_client_config" "current" {}

data "azurerm_subscription" "current" {}

# Random suffix for globally unique names
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
  numeric = true
}

# Local values
locals {
  environment    = var.environment
  location       = var.location
  project_name   = var.project_name
  resource_suffix = random_string.suffix.result
  
  # Common tags applied to all resources
  common_tags = merge(var.tags, {
    Environment = local.environment
    Project     = local.project_name
    ManagedBy   = "Terraform"
    CreatedOn   = timestamp()
  })

  # Naming convention
  naming = {
    resource_group       = "rg-${local.project_name}-${local.environment}-${local.location}"
    virtual_network      = "vnet-${local.project_name}-${local.environment}-${local.location}"
    key_vault           = "kv-${local.project_name}-${local.environment}-${local.resource_suffix}"
    storage_account     = "st${replace(local.project_name, "-", "")}${local.environment}${local.resource_suffix}"
    container_registry  = "cr${replace(local.project_name, "-", "")}${local.environment}${local.resource_suffix}"
    aks_cluster         = "aks-${local.project_name}-${local.environment}-${local.location}"
    log_analytics       = "log-${local.project_name}-${local.environment}-${local.location}"
    app_insights        = "appi-${local.project_name}-${local.environment}-${local.location}"
    ml_workspace        = "mlw-${local.project_name}-${local.environment}-${local.location}"
    cognitive_services  = "cog-${local.project_name}-${local.environment}-${local.location}"
  }
}

# Core Resource Group
resource "azurerm_resource_group" "main" {
  name     = local.naming.resource_group
  location = local.location
  tags     = local.common_tags
}

# Networking Module
module "networking" {
  source = "./modules/networking"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  vnet_name          = local.naming.virtual_network
  address_space      = var.vnet_address_space
  subnets            = var.subnets
  tags               = local.common_tags
}

# Security Module (Key Vault)
module "security" {
  source = "./modules/security"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  key_vault_name     = local.naming.key_vault
  tenant_id          = data.azurerm_client_config.current.tenant_id
  tags               = local.common_tags
  
  depends_on = [module.networking]
}

# Storage Module
module "storage" {
  source = "./modules/storage"
  
  resource_group_name   = azurerm_resource_group.main.name
  location             = azurerm_resource_group.main.location
  storage_account_name = local.naming.storage_account
  private_subnet_id    = module.networking.private_subnet_id
  tags                 = local.common_tags
  
  depends_on = [module.networking]
}

# Container Registry Module
module "container_registry" {
  source = "./modules/container_registry"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  registry_name      = local.naming.container_registry
  private_subnet_id  = module.networking.private_subnet_id
  tags               = local.common_tags
  
  depends_on = [module.networking]
}

# Monitoring Module
module "monitoring" {
  source = "./modules/monitoring"
  
  resource_group_name     = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  log_analytics_name     = local.naming.log_analytics
  app_insights_name      = local.naming.app_insights
  tags                   = local.common_tags
}

# AKS Module
module "aks" {
  source = "./modules/aks"
  
  resource_group_name        = azurerm_resource_group.main.name
  location                  = azurerm_resource_group.main.location
  cluster_name              = local.naming.aks_cluster
  subnet_id                 = module.networking.private_subnet_id
  log_analytics_workspace_id = module.monitoring.log_analytics_workspace_id
  acr_id                    = module.container_registry.registry_id
  environment               = local.environment
  tags                      = local.common_tags
  
  depends_on = [module.networking, module.container_registry, module.monitoring]
}

# LZAI Application Deployment (ACI or AKS based on deployment_type variable)
module "lzai_aci" {
  count  = var.deployment_type == "aci" ? 1 : 0
  source = "./modules/aci"

  container_group_name      = "${local.project_name}-aci"
  location                 = azurerm_resource_group.main.location
  resource_group_name      = azurerm_resource_group.main.name
  dns_name_label           = var.dns_name_label != null ? var.dns_name_label : "${local.project_name}-${local.environment}"
  
  acr_login_server         = module.container_registry.login_server
  acr_admin_username       = module.container_registry.admin_username
  acr_admin_password       = module.container_registry.admin_password
  
  image_tag                = var.image_tag
  environment              = local.environment
  
  backend_cpu              = var.aci_backend_cpu
  backend_memory           = var.aci_backend_memory
  frontend_cpu             = var.aci_frontend_cpu
  frontend_memory          = var.aci_frontend_memory
  redis_cpu                = var.aci_redis_cpu
  redis_memory             = var.aci_redis_memory
  
  log_analytics_workspace_id = module.monitoring.log_analytics_workspace_id
  
  tags = local.common_tags
  
  depends_on = [module.container_registry]
}

# AI Services Module
module "ai_services" {
  source = "./modules/ai_services"
  
  resource_group_name      = azurerm_resource_group.main.name
  location                = azurerm_resource_group.main.location
  ml_workspace_name       = local.naming.ml_workspace
  cognitive_services_name = local.naming.cognitive_services
  storage_account_id      = module.storage.storage_account_id
  key_vault_id           = module.security.key_vault_id
  app_insights_id        = module.monitoring.app_insights_id
  subnet_id              = module.networking.private_subnet_id
  tags                   = local.common_tags
  
  depends_on = [module.storage, module.security, module.monitoring, module.networking]
}
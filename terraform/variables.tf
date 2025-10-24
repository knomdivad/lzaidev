# Project Configuration
variable "project_name" {
  description = "Name of the project - used in resource naming"
  type        = string
  default     = "ailz"
  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.project_name))
    error_message = "Project name must contain only lowercase letters, numbers, and hyphens."
  }
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "East US"
}

variable "tags" {
  description = "Default tags to apply to all resources"
  type        = map(string)
  default = {
    Owner       = "AI Landing Zone Team"
    CostCenter  = "IT"
    Purpose     = "AI/ML Infrastructure"
  }
}

# Networking Configuration
variable "vnet_address_space" {
  description = "Address space for the virtual network"
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "subnets" {
  description = "Subnet configuration"
  type = map(object({
    address_prefixes = list(string)
    service_endpoints = optional(list(string), [])
    delegations = optional(list(object({
      name = string
      service_delegation = object({
        name    = string
        actions = list(string)
      })
    })), [])
  }))
  default = {
    public = {
      address_prefixes = ["10.0.1.0/24"]
      service_endpoints = ["Microsoft.Storage", "Microsoft.KeyVault"]
    }
    private = {
      address_prefixes = ["10.0.2.0/24"]
      service_endpoints = ["Microsoft.Storage", "Microsoft.KeyVault", "Microsoft.ContainerRegistry"]
    }
    data = {
      address_prefixes = ["10.0.3.0/24"]
      service_endpoints = ["Microsoft.Storage", "Microsoft.Sql"]
    }
    aks = {
      address_prefixes = ["10.0.4.0/24"]
      service_endpoints = ["Microsoft.Storage", "Microsoft.KeyVault"]
    }
  }
}

# AKS Configuration
variable "aks_node_count" {
  description = "Initial number of nodes in the AKS cluster"
  type        = number
  default     = 2
  validation {
    condition     = var.aks_node_count >= 1 && var.aks_node_count <= 10
    error_message = "AKS node count must be between 1 and 10."
  }
}

variable "aks_node_vm_size" {
  description = "VM size for AKS nodes"
  type        = string
  default     = "Standard_D4s_v3"
}

variable "aks_max_node_count" {
  description = "Maximum number of nodes for AKS autoscaling"
  type        = number
  default     = 5
}

variable "aks_min_node_count" {
  description = "Minimum number of nodes for AKS autoscaling"
  type        = number
  default     = 1
}

# Storage Configuration
variable "storage_account_tier" {
  description = "Storage account performance tier"
  type        = string
  default     = "Standard"
  validation {
    condition     = contains(["Standard", "Premium"], var.storage_account_tier)
    error_message = "Storage account tier must be Standard or Premium."
  }
}

variable "storage_replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
  validation {
    condition     = contains(["LRS", "GRS", "RAGRS", "ZRS"], var.storage_replication_type)
    error_message = "Storage replication type must be LRS, GRS, RAGRS, or ZRS."
  }
}

# AI Services Configuration
variable "cognitive_services_sku" {
  description = "SKU for Cognitive Services"
  type        = string
  default     = "S0"
}

variable "openai_deployment_models" {
  description = "OpenAI models to deploy"
  type = list(object({
    name         = string
    model_name   = string
    model_version = string
    scale_type   = string
    capacity     = number
  }))
  default = [
    {
      name          = "gpt-35-turbo"
      model_name    = "gpt-35-turbo"
      model_version = "0613"
      scale_type    = "Standard"
      capacity      = 10
    },
    {
      name          = "text-embedding-ada-002"
      model_name    = "text-embedding-ada-002"
      model_version = "2"
      scale_type    = "Standard"
      capacity      = 10
    }
  ]
}

# Security Configuration
variable "enable_private_endpoints" {
  description = "Enable private endpoints for services"
  type        = bool
  default     = true
}

variable "key_vault_soft_delete_retention_days" {
  description = "Number of days to retain soft-deleted Key Vault items"
  type        = number
  default     = 7
  validation {
    condition     = var.key_vault_soft_delete_retention_days >= 7 && var.key_vault_soft_delete_retention_days <= 90
    error_message = "Key Vault soft delete retention days must be between 7 and 90."
  }
}

# Monitoring Configuration
variable "log_analytics_retention_days" {
  description = "Number of days to retain logs in Log Analytics"
  type        = number
  default     = 30
  validation {
    condition     = var.log_analytics_retention_days >= 30 && var.log_analytics_retention_days <= 730
    error_message = "Log Analytics retention days must be between 30 and 730."
  }
}

variable "enable_diagnostic_logs" {
  description = "Enable diagnostic logs for resources"
  type        = bool
  default     = true
}

# LZAI Deployment Configuration
variable "deployment_type" {
  description = "Type of deployment (aci or aks)"
  type        = string
  default     = "aci"
  validation {
    condition     = contains(["aci", "aks"], var.deployment_type)
    error_message = "Deployment type must be aci or aks."
  }
}

variable "image_tag" {
  description = "Container image tag to deploy"
  type        = string
  default     = "latest"
}

variable "dns_name_label" {
  description = "DNS name label for ACI deployment"
  type        = string
  default     = null
}

# ACI Configuration
variable "aci_backend_cpu" {
  description = "CPU allocation for backend container in ACI"
  type        = string
  default     = "1"
}

variable "aci_backend_memory" {
  description = "Memory allocation for backend container in ACI (GB)"
  type        = string
  default     = "2"
}

variable "aci_frontend_cpu" {
  description = "CPU allocation for frontend container in ACI"
  type        = string
  default     = "0.5"
}

variable "aci_frontend_memory" {
  description = "Memory allocation for frontend container in ACI (GB)"
  type        = string
  default     = "1"
}

variable "aci_redis_cpu" {
  description = "CPU allocation for Redis container in ACI"
  type        = string
  default     = "0.5"
}

variable "aci_redis_memory" {
  description = "Memory allocation for Redis container in ACI (GB)"
  type        = string
  default     = "1"
}
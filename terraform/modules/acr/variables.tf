# Azure Container Registry Module Variables

variable "registry_name" {
  description = "Name of the Azure Container Registry"
  type        = string
  validation {
    condition     = can(regex("^[a-zA-Z0-9]+$", var.registry_name)) && length(var.registry_name) >= 5 && length(var.registry_name) <= 50
    error_message = "Registry name must be 5-50 alphanumeric characters."
  }
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "sku" {
  description = "SKU for the Azure Container Registry"
  type        = string
  default     = "Standard"
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.sku)
    error_message = "SKU must be Basic, Standard, or Premium."
  }
}

variable "admin_enabled" {
  description = "Enable admin user for ACR"
  type        = bool
  default     = true
}

variable "public_network_access_enabled" {
  description = "Enable public network access"
  type        = bool
  default     = true
}

variable "zone_redundancy_enabled" {
  description = "Enable zone redundancy (Premium SKU only)"
  type        = bool
  default     = false
}

variable "encryption_enabled" {
  description = "Enable encryption at rest"
  type        = bool
  default     = true
}

variable "retention_days" {
  description = "Number of days to retain untagged manifests"
  type        = number
  default     = 7
}

variable "retention_enabled" {
  description = "Enable retention policy"
  type        = bool
  default     = true
}

variable "trust_policy_enabled" {
  description = "Enable trust policy"
  type        = bool
  default     = false
}

variable "quarantine_policy_enabled" {
  description = "Enable quarantine policy"
  type        = bool
  default     = false
}

variable "log_analytics_workspace_id" {
  description = "Log Analytics workspace ID for diagnostics"
  type        = string
  default     = null
}

variable "enable_private_endpoint" {
  description = "Enable private endpoint for ACR"
  type        = bool
  default     = false
}

variable "private_endpoint_subnet_id" {
  description = "Subnet ID for private endpoint"
  type        = string
  default     = null
}

variable "private_dns_zone_ids" {
  description = "Private DNS zone IDs for private endpoint"
  type        = list(string)
  default     = []
}

variable "enable_build_task" {
  description = "Enable automated build task"
  type        = bool
  default     = false
}

variable "build_context_path" {
  description = "Build context path for ACR tasks"
  type        = string
  default     = "https://github.com/username/repo.git"
}

variable "github_token" {
  description = "GitHub personal access token for ACR tasks"
  type        = string
  default     = null
  sensitive   = true
}

variable "repository_url" {
  description = "Repository URL for source triggers"
  type        = string
  default     = null
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
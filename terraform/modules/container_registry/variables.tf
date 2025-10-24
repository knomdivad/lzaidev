variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "registry_name" {
  description = "Name of the container registry"
  type        = string
}

variable "sku" {
  description = "SKU for the container registry"
  type        = string
  default     = "Premium"
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.sku)
    error_message = "SKU must be Basic, Standard, or Premium."
  }
}

variable "admin_enabled" {
  description = "Enable admin user for the registry"
  type        = bool
  default     = false
}

variable "enable_private_endpoint" {
  description = "Enable private endpoint for container registry"
  type        = bool
  default     = true
}

variable "private_subnet_id" {
  description = "ID of the private subnet for private endpoint"
  type        = string
  default     = null
}

variable "virtual_network_id" {
  description = "ID of the virtual network"
  type        = string
  default     = null
}

variable "aks_kubelet_identity_principal_id" {
  description = "Principal ID of the AKS kubelet managed identity"
  type        = string
  default     = null
}

variable "retention_days" {
  description = "Number of days to retain images"
  type        = number
  default     = 30
}

variable "georeplications" {
  description = "List of additional regions for geo-replication"
  type = list(object({
    location                = string
    zone_redundancy_enabled = bool
  }))
  default = []
}

variable "webhooks" {
  description = "List of webhooks for the registry"
  type = list(object({
    name           = string
    service_uri    = string
    status         = string
    scope          = string
    actions        = list(string)
    custom_headers = map(string)
  }))
  default = []
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
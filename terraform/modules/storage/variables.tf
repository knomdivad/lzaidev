variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "storage_account_name" {
  description = "Name of the storage account"
  type        = string
}

variable "account_tier" {
  description = "Storage account performance tier"
  type        = string
  default     = "Standard"
  validation {
    condition     = contains(["Standard", "Premium"], var.account_tier)
    error_message = "Account tier must be Standard or Premium."
  }
}

variable "replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
  validation {
    condition     = contains(["LRS", "GRS", "RAGRS", "ZRS"], var.replication_type)
    error_message = "Replication type must be LRS, GRS, RAGRS, or ZRS."
  }
}

variable "enable_private_endpoint" {
  description = "Enable private endpoint for storage account"
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

variable "containers" {
  description = "Additional storage containers to create"
  type = map(object({
    access_type = string
  }))
  default = {}
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
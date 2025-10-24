# Variables for AI Services Module

variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
}

variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
}

variable "project_name" {
  description = "The name of the project for resource naming"
  type        = string
}

variable "environment" {
  description = "The environment name (dev, staging, prod)"
  type        = string
}

variable "ml_workspace_name" {
  description = "Name for the Azure Machine Learning workspace"
  type        = string
  default     = ""
}

variable "cognitive_services_name" {
  description = "Name for the Cognitive Services account"
  type        = string
  default     = ""
}

variable "cognitive_services_sku" {
  description = "SKU for the Cognitive Services account"
  type        = string
  default     = "S0"
  validation {
    condition     = contains(["F0", "S0", "S1", "S2", "S3", "S4"], var.cognitive_services_sku)
    error_message = "Cognitive Services SKU must be one of: F0, S0, S1, S2, S3, S4."
  }
}

variable "search_service_name" {
  description = "Name for the Azure AI Search service"
  type        = string
  default     = ""
}

variable "search_service_sku" {
  description = "SKU for the Azure AI Search service"
  type        = string
  default     = "standard"
  validation {
    condition     = contains(["free", "basic", "standard", "standard2", "standard3", "storage_optimized_l1", "storage_optimized_l2"], var.search_service_sku)
    error_message = "Search service SKU must be one of the valid Azure AI Search SKUs."
  }
}

variable "openai_models" {
  description = "Configuration for OpenAI model deployments"
  type = map(object({
    name          = string
    model_name    = string
    model_version = string
    capacity      = number
  }))
  default = {
    gpt35turbo = {
      name          = "gpt-35-turbo"
      model_name    = "gpt-35-turbo"
      model_version = "0613"
      capacity      = 30
    }
    gpt4 = {
      name          = "gpt-4"
      model_name    = "gpt-4"
      model_version = "0613"
      capacity      = 10
    }
    text_embedding_ada = {
      name          = "text-embedding-ada-002"
      model_name    = "text-embedding-ada-002"
      model_version = "2"
      capacity      = 30
    }
  }
}

variable "enable_private_endpoint" {
  description = "Enable private endpoints for AI services"
  type        = bool
  default     = true
}

variable "subnet_id" {
  description = "The subnet ID for private endpoints"
  type        = string
  default     = ""
}

variable "virtual_network_id" {
  description = "The virtual network ID for private DNS zone links"
  type        = string
  default     = ""
}

variable "app_insights_id" {
  description = "Application Insights resource ID for ML workspace"
  type        = string
}

variable "key_vault_id" {
  description = "Key Vault resource ID for ML workspace"
  type        = string
}

variable "storage_account_id" {
  description = "Storage account resource ID for ML workspace"
  type        = string
}

variable "tags" {
  description = "A map of tags to assign to resources"
  type        = map(string)
  default     = {}
}
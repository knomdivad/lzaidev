# Azure Container Instances Module Variables

variable "container_group_name" {
  description = "Name of the container group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "dns_name_label" {
  description = "DNS name label for the container group"
  type        = string
}

variable "acr_login_server" {
  description = "Azure Container Registry login server"
  type        = string
}

variable "acr_admin_username" {
  description = "Azure Container Registry admin username"
  type        = string
  sensitive   = true
}

variable "acr_admin_password" {
  description = "Azure Container Registry admin password"
  type        = string
  sensitive   = true
}

variable "image_tag" {
  description = "Container image tag to deploy"
  type        = string
  default     = "latest"
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

variable "backend_cpu" {
  description = "CPU allocation for backend container"
  type        = string
  default     = "1"
}

variable "backend_memory" {
  description = "Memory allocation for backend container (GB)"
  type        = string
  default     = "2"
}

variable "frontend_cpu" {
  description = "CPU allocation for frontend container"
  type        = string
  default     = "0.5"
}

variable "frontend_memory" {
  description = "Memory allocation for frontend container (GB)"
  type        = string
  default     = "1"
}

variable "redis_cpu" {
  description = "CPU allocation for Redis container"
  type        = string
  default     = "0.5"
}

variable "redis_memory" {
  description = "Memory allocation for Redis container (GB)"
  type        = string
  default     = "1"
}

variable "log_analytics_workspace_id" {
  description = "Log Analytics workspace ID for diagnostics"
  type        = string
  default     = null
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "cluster_name" {
  description = "Name of the AKS cluster"
  type        = string
}

variable "kubernetes_version" {
  description = "Kubernetes version"
  type        = string
  default     = "1.27.7"
}

variable "subnet_id" {
  description = "ID of the subnet for AKS"
  type        = string
}

variable "service_cidr" {
  description = "Service CIDR for AKS"
  type        = string
  default     = "10.1.0.0/16"
}

variable "dns_service_ip" {
  description = "DNS service IP for AKS"
  type        = string
  default     = "10.1.0.10"
}

variable "node_count" {
  description = "Initial number of nodes"
  type        = number
  default     = 2
}

variable "min_node_count" {
  description = "Minimum number of nodes"
  type        = number
  default     = 1
}

variable "max_node_count" {
  description = "Maximum number of nodes"
  type        = number
  default     = 5
}

variable "vm_size" {
  description = "VM size for system nodes"
  type        = string
  default     = "Standard_D4s_v3"
}

variable "ai_node_count" {
  description = "Number of AI workload nodes"
  type        = number
  default     = 2
}

variable "ai_max_node_count" {
  description = "Maximum number of AI nodes"
  type        = number
  default     = 10
}

variable "ai_node_vm_size" {
  description = "VM size for AI workload nodes"
  type        = string
  default     = "Standard_NC4as_T4_v3"
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "log_analytics_workspace_id" {
  description = "ID of the Log Analytics workspace"
  type        = string
}

variable "acr_id" {
  description = "ID of the Azure Container Registry"
  type        = string
  default     = null
}

variable "enable_http_application_routing" {
  description = "Enable HTTP application routing"
  type        = bool
  default     = false
}

variable "enable_private_cluster" {
  description = "Enable private cluster"
  type        = bool
  default     = true
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
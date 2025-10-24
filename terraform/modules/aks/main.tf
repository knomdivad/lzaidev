# AKS Module - Azure Kubernetes Service

# AKS Cluster
resource "azurerm_kubernetes_cluster" "main" {
  name                = var.cluster_name
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = "${var.cluster_name}-dns"
  kubernetes_version  = var.kubernetes_version
  
  # Network profile
  network_profile {
    network_plugin    = "azure"
    network_policy    = "azure"
    service_cidr      = var.service_cidr
    dns_service_ip    = var.dns_service_ip
    load_balancer_sku = "standard"
  }

  # Default node pool
  default_node_pool {
    name                = "system"
    node_count          = var.node_count
    vm_size             = var.vm_size
    vnet_subnet_id      = var.subnet_id
    type                = "VirtualMachineScaleSets"
    availability_zones  = ["1", "2", "3"]
    enable_auto_scaling = true
    min_count          = var.min_node_count
    max_count          = var.max_node_count
    os_disk_size_gb    = 128
    os_disk_type       = "Managed"
    
    # Node pool labels
    node_labels = {
      "nodepool-type" = "system"
      "environment"   = var.environment
      "workload"      = "system"
    }

    tags = var.tags
  }

  # Identity
  identity {
    type = "SystemAssigned"
  }

  # Azure AD integration
  azure_active_directory_role_based_access_control {
    managed            = true
    azure_rbac_enabled = true
  }

  # Add-ons
  oms_agent {
    log_analytics_workspace_id = var.log_analytics_workspace_id
  }

  # Enable Azure Policy
  azure_policy_enabled = true

  # HTTP application routing (for development)
  http_application_routing_enabled = var.enable_http_application_routing

  # Private cluster
  private_cluster_enabled = var.enable_private_cluster

  tags = var.tags
}

# Additional node pool for AI/ML workloads
resource "azurerm_kubernetes_cluster_node_pool" "ai_workload" {
  name                  = "aiworkload"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size              = var.ai_node_vm_size
  node_count           = var.ai_node_count
  vnet_subnet_id       = var.subnet_id
  availability_zones   = ["1", "2", "3"]
  enable_auto_scaling  = true
  min_count           = 1
  max_count           = var.ai_max_node_count
  os_type             = "Linux"
  os_disk_size_gb     = 256
  os_disk_type        = "Managed"

  # Node taints for AI workloads
  node_taints = ["workload=ai:NoSchedule"]

  # Node labels
  node_labels = {
    "nodepool-type" = "user"
    "workload"      = "ai"
    "environment"   = var.environment
  }

  tags = var.tags
}

# Role assignment for ACR integration
resource "azurerm_role_assignment" "aks_acr_pull" {
  count                = var.acr_id != null ? 1 : 0
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
}

# Role assignment for network contributor (for load balancer)
resource "azurerm_role_assignment" "aks_network_contributor" {
  scope                = var.subnet_id
  role_definition_name = "Network Contributor"
  principal_id         = azurerm_kubernetes_cluster.main.identity[0].principal_id
}
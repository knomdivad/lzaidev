output "virtual_network_id" {
  description = "ID of the virtual network"
  value       = azurerm_virtual_network.main.id
}

output "virtual_network_name" {
  description = "Name of the virtual network"
  value       = azurerm_virtual_network.main.name
}

output "subnet_ids" {
  description = "Map of subnet names to IDs"
  value       = { for k, v in azurerm_subnet.subnets : k => v.id }
}

output "public_subnet_id" {
  description = "ID of the public subnet"
  value       = azurerm_subnet.subnets["public"].id
}

output "private_subnet_id" {
  description = "ID of the private subnet"
  value       = azurerm_subnet.subnets["private"].id
}

output "data_subnet_id" {
  description = "ID of the data subnet"
  value       = azurerm_subnet.subnets["data"].id
}

output "aks_subnet_id" {
  description = "ID of the AKS subnet"
  value       = azurerm_subnet.subnets["aks"].id
}

output "nat_gateway_id" {
  description = "ID of the NAT Gateway"
  value       = azurerm_nat_gateway.main.id
}

output "public_nsg_id" {
  description = "ID of the public network security group"
  value       = azurerm_network_security_group.public.id
}

output "private_nsg_id" {
  description = "ID of the private network security group"
  value       = azurerm_network_security_group.private.id
}
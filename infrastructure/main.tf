terraform {
  backend "azurerm" {
  }
}

# Configure the Azure Provider
provider "azurerm" {
  version             = "~>2.7.0"
  features {}
}

locals {  
  environment_name    = var.environment_name
  topic_name = "DatasetProvisioned" 
}

data "azurerm_resource_group" "shared" {
  name 				  = "shared-${var.environment_name}"
}

data "azurerm_servicebus_namespace" "servicebus" {
  name                = "dpdomainevents-servicebus-${var.environment_name}"
  resource_group_name = data.azurerm_resource_group.shared.name  
}

resource "azurerm_servicebus_subscription" "sub" {
  name                = "DataCatalog-API"
  resource_group_name = data.azurerm_resource_group.shared.name
  namespace_name      = data.azurerm_servicebus_namespace.servicebus.name
  topic_name          = local.topic_name
  lock_duration       = "PT5M" #5 minutes, 30 seconds default is too short for processing
  max_delivery_count  = 3
}
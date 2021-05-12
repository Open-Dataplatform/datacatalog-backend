# The following variable should be defined in the 'core.tfvars' files within the 'environments/test|preprod|prod' folders
# If working on your development computer, the variables can be set as environment variables using 'TF_VAR_resourcename'-format (more info here: https://www.terraform.io/docs/commands/environment-variables.html)

# Name of the environment hosting the infrastructure
variable "environment_name" {
    default = ""
}

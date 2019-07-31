# viacode-incident-management
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FmopeLantebyx%2Fviacode-incident-management%2Fdev%2Fazuredeploy.json" target="_blank">
<img src="https://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FmopeLantebyx%2Fviacode-incident-management%2Fdev%2Fazuredeploy.json" target="_blank">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.png"/>
</a>

The VIAcode Azure Incident Management System provides a free full-featured incident management solution for Azure users.


`Tags: VIAcode, Azure Managed application, Incident Management, Docker`

## Solution overview and deployed resources

This is an overview of the solution

The following resources are deployed as part of the solution

+ **Azure Container Registry**: Docker image registry

## Deployment steps

You can click the "deploy to Azure" button at the beginning of this document or follow the instructions for command line deployment using the scripts in the root of this repo.


To deploy this template using the scripts from the root of this repo:

```PowerShell
.\Deploy-AzureResourceGroup.ps1 -ResourceGroupLocation 'eastus' -ArtifactsStagingDirectory '101-container-registry'
```
```bash
azure-group-deploy.sh -a '101-container-registry' -l eastus -u
```

## Usage

## Login to your registry

Follow [this documentation](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-authentication) for authenticate your docker client to your container registry.

#### Push images to your registry

For pushing docker images on your registry, follow [this documentation](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-docker-cli)

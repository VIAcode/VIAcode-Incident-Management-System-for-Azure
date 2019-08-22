# viacode-incident-management
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FVIAcode%2Fviacode-incident-management%2Fmaster%2Fazuredeploy.json" target="_blank">
<img src="https://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FVIAcode%2Fviacode-incident-management%2Fmaster%2FZammad%2FAppService%2FmainTemplate.json" target="_blank">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.png"/>
</a>

VIAcode Azure Incident Management System

`Tags: VIAcode, Azure Managed application, Incident Management, Docker`

## Solution overview and deployed resources

The VIAcode Azure Incident Management System provides a free full-featured incident management solution for Azure users.

The following resources are deployed as part of the solution

+ **Linux App Service**: Incident Management App Service
+ **Automation Account**: setup authorization runbook
+ **KeyVault**: Storage for IMS admin credentials
+ **Dashboard**: Dashboard
+ **Action Group**: Trigger for IMS connector
+ **Alert Rules**: Default set of monitoring alerts
+ **Azure function**: IMS connector

## Deployment

+ Click the "Deploy to Azure" button at the beginning of this document to deploy Azure Managed App Definition to azure portal and then start the Deploy from definition on portal.
+ Or follow [this documentation](https://github.com/VIAcode/viacode-incident-management/blob/master/VIAcode%20Azure%20Incident%20Management%20System%20deployment%20and%20configuration%20guide.md) to build the solution from source code and deploy manually

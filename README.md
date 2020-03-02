# VIAcode-Incident-Management-System-for-Azure

The VIAcode Incident Management System for Azure is a robust incident management tool that fully integrates into an Azure environment helping to prioritize, simplify and streamline management of your Azure operations

`Tags: VIAcode, Azure Managed application, Incident Management, Docker`

## Solution overview and deployed resources

The VIAcode Incident Management System for Azure provides a free full-featured incident management solution for Azure users.

The following resources are deployed as part of the solution

+ **Linux App Service**: Incident Management App Service
+ **Automation Account**: Setup authorization runbook
+ **KeyVault**: Storage for IMS admin credentials
+ **Dashboard**: Dashboard
+ **Action Group**: Trigger for IMS connector
+ **Alert Rules**: Default set of monitoring alerts
+ **Function App**: IMS connector
+ **Service Bus**: Service Bus queue which is used to create new tickets

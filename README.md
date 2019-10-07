# VIAcode-Incident-Management-System-for-Azure

The VIAcode Incident Management System for Azure is a robust incident management tool that fully integrates into an Azure environment helping to prioritize, simplify and streamline management of your Azure operations. [Read more](https://www.viacode.com/viacode-incident-management-system/?utm_source=product&utm_medium=web&utm_campaign=VIMS&utm_content=githubreadmevims)

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
+ **Azure Function**: IMS connector

## Deployment

+ Click the "Deploy to Azure" button at the beginning of this document to deploy Service catalog managed application definition to Azure portal. After that deploy VIAcode Incident Management System for Azure from the definition on Azure portal.
+ Or follow [this documentation](https://github.com/VIAcode/VIAcode-Incident-Management-System-for-Azure/blob/master/VIAcode%20Incident%20Management%20System%20for%20Azure%20deployment%20and%20сonfiguration%20guide.md) to build the solution from source code and deploy manually.
+ By default VIAcode Incident Management System for Azure will create tickets for alerts only from a subscription that you deploy it to.
To connect another subscription to VIAcode Incident Management System for Azure you will have to [install Alert Connector](https://portal.azure.com/#create/viacode_consulting-1089577.viacode-vims-previewvims-paid) to that subscription.
You can automatically deploy alert connector to multiple subscriptions.
To do it run ConnectorAutoDeployment\DeployConnectorToMultipleSubscripitons.ps1 script and follow instructions.
By deploying alert connector via this script you automatically accept [terms of use](https://www.viacode.com/VIAcode%20Marketplace%20Terms%20of%20Use.pdf?utm_source=product&utm_medium=web&utm_campaign=VIMS&utm_content=githubreadmetermofuse), [privacy policy](https://www.viacode.com/VIAcode%20Marketplace%20Privacy%20Policy.pdf?utm_source=product&utm_medium=web&utm_campaign=VIMS&utm_content=githubreadmepolicy) and [Azure Marketplace Terms](https://azure.microsoft.com/support/legal/marketplace-terms/).

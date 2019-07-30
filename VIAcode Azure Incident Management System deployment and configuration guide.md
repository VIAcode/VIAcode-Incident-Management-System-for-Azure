#VIAcode Azure Incident Management System deployment and configuration guide
<!-- TOC -->
- [Deployment of VIAcode Azure Incident Management System](#deployment-of-viacode-azure-incident-management-system)
  - [Get source code](#get-source-code)
  - [Build ITSMConnector](#build-itsmconnector)
  - [Pack Azure Function](#pack-azure-function)
  - [Prepare package](#prepare-package)
  - [Deploy Managed Application Definition](#deploy-managed-application-definition)

- [Configuration of VIAcode Azure Incident Management System](#configuration-of-viacode-azure-incident-management-system)
  - [Basics](#basics)
  - [Settings](#settings)
  - [Azure AD Integration](#azure-ad-integration)
    - [Purpose of an app registration](#purpose-of-an-app-registration)
  - [Review and create](#review-and-create)
  - [Redirect URI for Azure AD Integration](#redirect-uri-for-azure-ad-integration)
  - [First Sign in](#first-sign-in)

- [Additional information](#additional-information)
  - [Steps to create a new App registration in Azure AD](#steps-to-create-a-new-app-registration-in-azure-ad)
  - [Email configuration](#email-configuration)
  
<!-- TOC END -->

# Deployment of VIAcode Azure Incident Management System
## Get source code
Download or clone viacode-incident-management.

## Build ITSMConnector
You have to build ITSMConnector.sln to get deployable Azure Function.
To do it you can either organize a build in Azure DevOps or build it locally.

![Build ITSMConnector](./media/Build&#32;ITSMConnector.png)

For more information regarding Azure Functions please use [this reference](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio).
 
## Pack Azure Function
Use `Compress-Archive` PowerShell command to pack built azure function.

**Example**
`Compress-Archive -Path "D:\!GIT\ITSMConnector\ITSMConnector\publish_output\*.json" -DestinationPath "D:\!GIT\ITSMConnector\ITSMConnector\publish_output\zammadconnector.zip"`

## Prepare package
Pack "linkedTemplates" folder, "CreateAdminAndEnableAuthViaOffice365Runbook.ps1", "createUiDefinition.json", "mainTemplate.json", "viewDefinition.json" and zammadconnector.zip into "itsm-z-free.zip" package.

## Deploy Managed Application Definition
- Upload the package into an Azure blob or another accessible location
- Execute the script below after filling [parameters]:

```
// Get Storage account where "itsm-z-free.zip" is stored in a blob
$storageAccount = Get-AzStorageAccount -ResourceGroupName "[RG name]" -Name "[Storage account]" 

// Get Context of the Storage Account
$ctx = $storageAccount.Context

// Get the blob with "itsm-z-free.zip"
$blob = Get-AzStorageBlob -Container "[Container Name]" -Blob "[itsm-z-free.zip]" -Context $ctx

//Get owner ID
$ownerID=(Get-AzRoleDefinition -Name Owner).Id

//Get user ID
$userId = (Get-AzADUser -UserPrincipalName '[User principal name]').Id

//Get contributor ID
$contributorID=(Get-AzRoleDefinition -Name Contributor).Id

//Get AD Service Principal ID by name
$appId = (Get-AzADServicePrincipal -ServicePrincipalName 'a0778614-7329-40be-a67b-e51cd7dd03b0').Id

//Get user ID
New-AzManagedApplicationDefinition `
  -Name "VIAcode Azure Incident Management System" `
  -Location "Central US" `
  -ResourceGroupName "VIAcodeAIMS" `
  -LockLevel ReadOnly `
  -DisplayName "VIAcodeAIMS" `
  -Description "VIAcodeAIMS" `
  -Authorization "${userId}:$ownerID","${appId}:$contributorID" `
  -PackageFileUri $blob.ICloudBlob.StorageUri.PrimaryUri.AbsoluteUri  
```

# Configuration of VIAcode Azure Incident Management System
When you have Managed App Definition is installed run it to create VIAcode Azure Management System

## Basics 
![Basics](./media/Basics.png)
- Choose a subscription to deploy the management application
- Create a new Resource Group
- Select a location
- Press "Next : Settings >" button

## Settings 
![Settings](./media/Settings.png) 
- Set a nameprefix for the resources
- Specify email address for VIAcode Azure Incident Management System administrator
- Set password
- Press "Next : Azure AD Integration >" button

## Azure AD Integration
To enable Azure AD Integration you have to specify “Azure AD Application Registration ID" and "Secret”. To create a new App registration please look at the [Steps to create a new App registration in Azure AD](#steps-to-create-a-new-app-registration-in-azure-ad).
Select "Disabled" if you do not need Azure Active Directory integration.
![Azure AD Integration](./media/Azure&#32;AD&#32;Integration.png)
- Either Disable Azure AD integration or Enable
- Set "Azure AD Application Registration ID" 
- Set "Secret"
- Press "Next : Review + create >" button

### Purpose of an app registration
It is used to integrate VIAcode Azure Incident Management System with Azure AD and Office 365.
Using Azure App, we can generate the token to authenticate the application.
When Azure App is created we can get the "Application (client) ID" and "Secret".
For more information read [the following.](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)

## Review and create
![Review + create](./media/Review&#32;+&#32;create.png)
- Agree to the terms and conditions
- Press "Create" button

## Redirect URI for Azure AD Integration
When deployment finishes (it usually takes up to 15 minutes to complete) you will have to configure redirect URI to enable Azure Active Directory integration.

**Step 1**
In the left-hand navigation pane, select the Azure Active Directory service, and then select App registrations > [Name of the App registration you used to install VIAcode Azure Incident Management System].

**Step 2**
Click on Redirect URIs link.
![Redirect URIs link](./media/Click&#32;on&#32;Redirect&#32;URIs&#32;link.png)

**Step 3**

Configure Redirect URI
- TYPE - Web
- REDIRECT URI - https://[App Service Address]/auth/microsoft_office365/callback
Note: The [App Service Address] can be copied from "Parameters and Outputs" of the installed managed application.
![Configure Redirect URI](/.attachments/Configure&#32;Redirect&#32;URI.png)
Final string looks like this "https://viaaims-milyddjpnf8sw.azurewebsites.net/auth/microsoft_office365/callback"
- Save

## First Sign in
![First Sign in](./media/First&#32;Sign&#32;in.png)
Open VIAcode Incident Management System dashboard and click on the link to get to the system.

![Sign in using credentials](./media/Sign&#32;in&#32;using&#32;credentials.png)
Sign in using credentials you entered for VIAcode Azure Incident Management System administrator.

#Additional information
## Steps to create a new App registration in Azure AD
Follow the below-listed steps to register the application.

**Step 1**
Log into the Azure portal using your Azure account.
URL - https://portal.azure.com/

**Step 2**
In the left-hand navigation pane, select the Azure Active Directory service, and then select App registrations > New registration.

**Step 3**
When the Register an application page appears, enter your application's registration information:
- Name: Enter a meaningful application name that will be displayed to users of the app. (VIAcode Azure Incident Management System)
- Supported account types: Select which accounts you would like your application to support. (Accounts in this organizational directory only (#ADname#))
- Redirect URI (optional): Leave it for now. It should be specified later when the managed application is installed.
- Register.

**Step 4**
In the menu blade select Certificates & secrets > New client secret.

**Step 5**
When the Add a client secret page appears, specify Description and Expiration period.

**Step 6**
Copy the secret to clipboard. 
Use it as "Secret" in "Create VIAcode Incident management System" wizard.
![Copy Secret](./media/Create&#32;VIAcode&#32;Incident&#32;management&#32;System.png)

**Step 7**
Navigate to the overview of the app registration and copy Application (client) ID.  
Use it as "Azure AD Application Registration ID" in "Create VIAcode Incident management System" wizard.
![copy Application (client) ID](./media/copy&#32;Application&#32;(client)&#32;ID.png)

## Email configuration
To configure email notification please instructions on [Zammad docs](https://admin-docs.zammad.org/en/latest/channels-email.html)







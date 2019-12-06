# VIAcode Incident Management System for Azure deployment and configuration guide
<!-- TOC -->
- [Before you begin](#before-you-begin)
  - [Deploy from Azure Marketplace](#deploy-from-azure-marketplace)
    - [Pricing](#pricing)
- [Configuration of VIAcode Incident Management System for Azure](#configuration-of-viacode-incident-management-system-for-azure)
  - [Basics](#basics)
  - [Settings](#settings)
    - [Purpose of an App registration](#purpose-of-an-app-registration)
  - [Review and create](#review-and-create)
  - [Redirect URI for Azure AD Integration](#redirect-uri-for-azure-ad-integration)
  - [Enable Azure AD Integration after installation](#enable-azure-ad-integration-after-installation)
  - [Alert state backward synchronization](#alert-state-backward-synchronization)
    - [Overview](#overview)
    - [How to setup](#how-to-setup)
  - [First Sign in](#first-sign-in)
  - [Email configuration](#email-configuration)
    - [Notes](#notes)
  - [Activity Log Alert message](#activity-log-alert-message)

- [Technical details](#technical-details)
  - [Supported alert types](#supported-alert-types)

- [Additional information](#additional-information)
  - [Steps to create a new App registration in Azure AD](#steps-to-create-a-new-app-registration-in-azure-ad)
    
- [Configuration of VIAcode Alert Connector Premium](#configuration-of-viacode-alert-connector-premium)
  - [Premium Basics](#premium-basics)
  - [Premium Settings](#premium-settings)
  - [Premium Review and create](#premium-review-and-create)
  
<!-- TOC END -->

## Before you begin

Verify that your account user type is not Guest in chosen tenant.

- Sign in to the [Azure portal](https://portal.azure.com/).
- Select "Azure Active Directory", select "Users".

![Guest type account](./media/Guest&#32;account.png)

[Guest](https://docs.microsoft.com/en-us/azure/active-directory/b2b/user-properties) accounts have limited permissions. Deployment under a guest account will fail.

## Deploy from Azure Marketplace

- [Navigate](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/viacode_consulting-1089577.viacode-itsm-z?tab=Overview) to Microsoft Azure Marketplace and find "VIAcode Incident Management System for Azure" offer.
![Azure Market Place](./media/AzureMarketPlace.png)
- Press "GET IT NOW" button.
- Select "VIAcode Incident Management System for Azure" software plan and press "Continue".

You will be taken to Azure Portal to complete installation:
![Azure Portal](./media/AzurePortalOfferProfile.png)
- Select "VIAcode Incident Management System for Azure" software plan.
- Press "Create".

### Pricing

The total cost of running VIAcode Incident Management System on Azure is a combination of the selected software plan and cost of the Azure infrastructure on which you will be running it. The Azure infrastructure cost might vary with regards to the region, type of subscription and other discounts.

## Configuration of VIAcode Incident Management System for Azure

After you have selected "VIAcode Incident Management System for Azure" software plan you need to configure it.

## Basics

![Basics](./media/Basics.png)

- Choose a subscription to deploy the management application.
- Create a new Resource Group.
- Select a region.
- Provide a name for your application's managed resource group.
- Press "Next : Settings >" button.

## Settings

To enable Azure AD Integration you have to specify Azure AD Application Registration ID and Secret. To create a new App registration see [Steps to create a new App registration in Azure AD](#steps-to-create-a-new-app-registration-in-azure-ad).

![Azure AD Integration](./media/Azure&#32;AD&#32;Integration.png)

- Either "Enable" Azure AD integration or leave it "Disabled".
- Set Azure AD Application Registration ID.
- Set Secret.
- Press "Next : Review + create >" button.

### Purpose of an App registration

It is used to integrate VIAcode Incident Management System for Azure with Azure AD and Office 365.
Using Azure App, we can generate the token to authenticate the application.
When Azure App is created we can get the Application (client) ID and Secret.
For more information see [Quickstart: Register an application with the Microsoft identity platform.](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)

## Review and create

![Review + create](./media/Review&#32;+&#32;create.png)

- Agree to the terms and conditions.
- Press "Create" button.

## Redirect URI for Azure AD Integration

When deployment finishes (it usually takes up to 15 minutes to complete) you will have to configure redirect URI to enable Azure Active Directory integration.

**Step 1**
In the left-hand navigation pane, select the "Azure Active Directory" service, then select "App registrations" and "[Name of the App registration you used to install VIAcode Incident Management System for Azure]."

**Step 2**
Click on "Redirect URIs" link.

![Redirect URIs link](./media/Click&#32;on&#32;Redirect&#32;URIs&#32;link.png)

**Step 3**
Configure Redirect URI.

- TYPE - Web.
- REDIRECT URI - `https://[App Service Address]/auth/microsoft_office365/callback`.

Note: The [App Service Address] can be copied from Parameters and Outputs of the installed managed application.

![Configure Redirect URI](./media/Configure&#32;Redirect&#32;URI.png)

Final string looks like `https://viaaims-milyddjpnf8sw.azurewebsites.net/auth/microsoft_office365/callback`.

- "Save."

### Enable Azure AD Integration after installation

To enable Office 365 login after you installed VIAcode Incident Management System for Azure with AD Integration disabled follow these steps:

1. Enable HTTPS in System - Base and Make sure Fully Qualified Domain Name is identical to your real URL.

![Check HTTPS enabled](./media/Https&#32;config&#32;enabled.png)

2. Select Security - Third-party Applications and enable checkbox Authentication via Office 365, then fill App ID and App Secret according to created application.

![Configure Office365](./media/Auth&#32;Office&#32;365&#32;config.png)


## Alert state backward synchronization

### Overview

VIAcode Incident Management System for Azure provides an alert state backward synchronization mechanism. Whenever an incident created based on an Azure alert or closed, the alert is closed automatically.

### How to setup

In order to configure alert state synchronization please assign the VIAcode Incident Management System for Azure Function App Monitoring Contributor Role for your subscription in Azure portal.

- Click on "Subscriptions."
- Select the subscription where VIAcode Incident Management System for Azure is deployed.
- Click "Access control (IAM)."
- "Add" > "Add role assignment."

  - Role: 'Monitoring Contributor'.
  - Assign access to: 'Azure AD user, group, or service principal'.
  - Subscription: Your Subscription.
  - Select: Function app name for VIAcode Incident Management System for Azure.*
  - "Save."

 *(Function app name equals connectorName, can be copied from 'Parameters and Outputs' of the installed managed application)
  ![Connector name](./media/connectorName.png)  

You can also execute the following PS script:

```powershell
New-AzRoleAssignment -ObjectId (Get-AzADServicePrincipal -SearchString '{CONNECTOR_NAME}').Id -RoleDefinitionName 'Monitoring Contributor' -Scope '/subscriptions/{SUBSCRIPTION_ID}';
```

SUBSCRIPTION_ID - ID of a monitored subscription.  
CONNECTOR_NAME - The CONNECTOR_NAME can be copied from 'Parameters and Outputs' of the installed managed application.

If you have multiple subscriptions, execute the script for each of them.

## First Sign in

![First Sign in](./media/First&#32;Sign&#32;in.png)

- Open VIAcode Incident Management System for Azure dashboard and click on the link to get to the system.

![Sign in using credentials](./media/Sign&#32;in&#32;using&#32;credentials.png)

- Sign in using credentials you entered for VIAcode Incident Management System for Azure administrator.

## Email configuration

When you Log in to VIAcode Incident Management System for Azure as administrator please configure email notification.

![Notification Sender](./media/Notification&#32;Sender.png)

- Click "Cogwheel" in the lest bottom corner.
- Click "Email" in Channels section.
- Click "Settings".
- Enter sender name and email in angle brackets in Notification Sender section like it is shown in the screenshot.
- Click "Submit".

![Email Notification](./media/Email&#32;Notification.png)

- Click "Accounts".
- Click "Edit" in Email Notification section.
- Select "SMTP - configure your own outgoing SMTP settings" in Send Mails via.
- Fill Host with `outlook.office365.com` for office 365 accounts.
- Fill User with the account you will you use for email notification.
- Fill Password.
- Default port for SMTP is 587.
- Click "Continue" button.

![New email Accounts and Experts](./media/New&#32;email&#32;Accounts&#32;and&#32;Experts.png)

- Click "New" button in Email Accounts section.
- Fill Organization & Department Name.
- Fill Email with the account you will you use for email notification.
- Select "Users" in Destination Group.
- Click "Experts" link.

![Email Inbound](./media/Email&#32;Inbound.png)

- Select "IMAP" Type.
- Fill Host with `outlook.office365.com` for office 365 accounts.
- Fill User with the account you will you use for email notification.
- Fill Password.
- Default port for IMAP is 993.
- Click "Continue" button.

![Email Outbound](./media/Email&#32;Outbound.png)

- Check settings.
- Default port for SMTP is 587.
- Click "Continue" button.

### Notes

Email configuration is required for enabling following functionality:

- Send system account notifications - signup, password reset, password change.
- Register incoming emails as tickets (incoming mail).
- Send notifications about new tickets to the agents, send reminders.
- Use triggers to inform clients about ticket creation, status changes.
- Agent communication with clients by email (using built in web or regular mail client).

It is important to note that after deployment, by default, email profiles are not created. Client has to set up email for VIAcode Incident Management System for Azure manually.

In general, VIAcode Incident Management System for Azure best practice is to create new dedicated empty mail account in company mail system. VIAcode Incident Management System for Azure usually can auto-detect settings, so nothing apart email and password is required.

- Google blocks access to the email from non-google devices by default, so you have to adjust security settings.
- Do not add your work email, VIAcode Incident Management System for Azure will register all emails there and send notification response to each.
- Do not use shared mailbox in Office 365, VIAcode Incident Management System for Azure can not log into it.
- Do not use mailbox that forwards mails, VIAcode Incident Management System for Azure will be unable to read its own test email.

For current email set up documentation see [e-mail](https://zammad-admin-documentation.readthedocs.io/en/latest/channels-email.html).

## Activity Log Alert message

At the time of this writing description didn't work for Activity Log Alerts in Azure.
Because of this alert message hasn't been filled for tickets created from Activity Log Alerts.
Workaround that fills alert message for tickets created from Activity Log Alerts has been implemented.
Alert message is obtained from corresponding alert rule's description.
However to get it up and working you need to assign at least Reader role to connector function app.

In order to do so:

- Click on "Subscriptions."
- Select the subscription where VIAcode Incident Management System for Azure is deployed.
- Click "Access control (IAM)."
- "Add" > "Add role assignment."

  - Role: 'Reader'.
  - Assign access to: 'Function App'.
  - Subscription: Your Subscription.
  - Select: Function app name for VIAcode Incident Management System for Azure.*
  - "Save."

 *(Function app name equals connectorName, can be copied from 'Parameters and Outputs' of the installed managed application)
  ![Connector name](./media/connectorName.png)  

You can also execute the following PS script:

```powershell
New-AzRoleAssignment -ObjectId (Get-AzADServicePrincipal -SearchString '{CONNECTOR_NAME}').Id -RoleDefinitionName Reader -Scope '/subscriptions/{SUBSCRIPTION_ID}';
```

SUBSCRIPTION_ID - ID of a monitored subscription.  
CONNECTOR_NAME - The CONNECTOR_NAME can be copied from 'Parameters and Outputs' of the installed managed application.

If you have multiple subscriptions, execute the script for each of them.

## Technical details

### Supported alert types

VIAcode Incident Management System for Azure can process the following Azure alert types:

- Metric Alerts

  - Platform

- Log Alerts

  - Log Analytics
  - Application Insights

- Activity Log Alerts

  - Activity Log - Administrative
  - Activity Log - Policy
  - Activity Log - Autoscale
  - Activity Log - Security
  - Service Health

Please be informed, that alerts generated by Azure Cost Management currently are not supported.

## Additional information

## Steps to create a new App registration in Azure AD

Follow the below-listed steps to register the application.

**Step 1**
Log in to the Azure portal using your Azure account.
URL - <https://portal.azure.com/>.

**Step 2**
In the left-hand navigation pane, select the "Azure Active Directory" service, and then select" App registrations" > "New registration."

**Step 3**
When the Register an application page appears, enter your application's registration information:

- Name: Enter a meaningful application name that will be displayed to users of the app. (VIAcode Incident Management System for Azure.)
- Supported account types: Select which accounts you would like your application to support. (Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)).
- Redirect URI (optional): Leave it for now. It should be specified later when the managed application is installed.
- "Register."

**Step 4**
In the menu blade select "Certificates & secrets" > "New client secret."

**Step 5**
When the Add a client secret page appears, specify Description and Expiration period.

**Step 6**
Copy the secret to clipboard.
Use it as Secret in Create VIAcode Incident Management System for Azure wizard.

![Copy Secret](./media/Create&#32;VIAcode&#32;Incident&#32;management&#32;System.png)

**Step 7**
Navigate to the overview of the App registration and copy Application (client) ID.  
Use it as Azure AD Application Registration ID in Create VIAcode Incident Management System for Azure wizard.

![copy Application (client) ID](./media/copy&#32;Application&#32;(client)&#32;ID.png)

**Step 8**
Configure App registration authentication.

Go to Authentication blade.
Switch the radio button under 'Supported account types' to Multitenant:

![Configure Redirect URI](./media/MultitenantConfiguration.png)

## Configuration of VIAcode Alert Connector Premium

Find VIAcode Incident Management System for Azure app in Azure Marketplace and click "GET IT NOW".

## Premium Basics

![Basics](./media/Premium&#32;basics.png)

- Choose a subscription to deploy the managed application.
- Create a new Resource Group.
- Select a region.
- Provide a name for your application's managed resource group.
- Press "Next : Settings >" button.

## Premium Settings

![Settings](./media/Premium&#32;settings.png)

- Specify id of a subscription where VIAcode Incident Management System for Azure to which you want to connect selected subscription is deployed to.
- Specify name of a managed resource group of that VIAcode Incident Management System for Azure.
- Specify name of a connector function app of that VIAcode Incident Management System for Azure.
You can find it in output of a corresponding managed application under name connectorName:
  ![Connector name](./media/connectorName.png)  
- Press "Next : Review + create >" button.

## Premium Review and create

![Review + create](./media/Premium&#32;review&#32;+&#32;create.png)

- Agree to the terms and conditions.
- Press "Create" button.

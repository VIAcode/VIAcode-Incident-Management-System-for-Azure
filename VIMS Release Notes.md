# VIAcode Incident Management System for Azure Release Notes

<!-- TOC -->
- [VIAcode Incident Management System for Azure 1.3.0](#viacode-incident-management-system-for-azure-130)
- [VIAcode Incident Management System for Azure 1.2.5](#viacode-incident-management-system-for-azure-125)
- [VIAcode Incident Management System for Azure initial release](#viacode-incident-management-system-for-azure-initial-release)  
<!-- TOC END -->

## VIAcode Incident Management System for Azure 1.3.0

### Features

- Added support for automatic ticket delegation to Azure DevOps
- Added alert processing retry in case of any errors
- Added tracking of changes to Azure resources
- Added Teams bot allowing to manage tickets
- Added "My delegated Tickets" overview for customers
- Made that roles should be assigned to managed application instead of specific resources in managed resource group
- Made that states of alerts generated before ticket creation which are related to that ticket are synchronized
- Added article creation in ticket on alert repeats
- Enabled HTTPS only for web application
- Added synchronization of states for merged tickets

### Bug-fixes

- Fixed VIAcode Insights ticket look
- Fixed backward synchronization in case when states of tickets created from alerts that are more than one month old are changed
- Hidden technical tags
- Ticket creation optimization
- Fixed article subjects
- Fixed ticket duplication
- Fixed VIAcode Insights Feed
- Fixed ticket repeat count
- Added timezone specification to times in ticket articles
- Made that not only the first page of alerts is closed when corresponding ticket is closed

## VIAcode Incident Management System for Azure 1.2.5

### Features

- Simplified installation process. By default Admin user name is 'admin' and password is 'admin'. Default prefix for resources is 'vims'.
- Restyled formating of tickets created based on Azure Monitor alerts.Notification emails has same new clear format.
- A new Repeat Count property was added to every ticket, it displays the number of alert repeats.
- Azure Alert status changes to Acknowledged when an incident created in VIMS changes its state to Open.
- A new default group 'Incoming' added, it replaces 'Users'.
- A new designated user 'Azure Monitor Connector' was added. It connects VIMS to Azure Monitor. All new tickets are created by this user. By default 'Azure Monitor Connector' user is Customer for such tickets.
- By default VIMS has 'Customer' and 'Default SRE Provider' shared organizations.
- Added states Delegated and Pending Review for tickets. Customer can set ticket into Pending Review state in place of closing the ticket.
- A new role 'Connector' was added. It has minimal set of privileges just to create and update tickets. Agent role has full access to Incoming group and it is an editor of Knowledge Base. Customer role has access to Incoming group.
- An avatar for Azure Monitor Connector was added.
- 'My assigned Tickets', 'Open' and 'Unassigned&Open' overviews now displays delegated and pending review tickets. 'My delegated Tickets' overview was added.
- Customer now receives notifications about any updates in it's tickets.
- Azure Alert severity now affects ticket priority.
- A new [repository](https://github.com/VIAcode/VIAcode-Incident-Management-System) for VIMS.

### Bug-fixes

- Dates in tickets are in 12-hour with AM/PM indication.
- Installation wizard lists only the right locations.
- There are no duplicates and lost alerts anymore.
- Tickets can be created manually.
- An Azure Monitor ticket can be created even if Threshold field from Alert Rule is decimal.
- Activity Log alerts now can be automatically closed after ticket's closing in VIMS.
- Broken links are no longer displayed in tickets when an Azure Alert does not have a Resource Group or a resource.
- Minor bug-fixes.

## VIAcode Incident Management System for Azure initial release

VIAcode Incident Management System is a cloud-born ticketing system that combines a powerful incident management engine with out-of-the-box integration with Azure platform. It creates a process around alerts, recommendations and threats in order to foster individual and organizational accountability. It helps visualize operational maturity and compliance.

### Incident Management

- Ability to set SLA for incident types.
- Custom attributes.
- Full text search.
- Knowledge base of Azure best practices.

### Azure integrations

- Ability to escalate incidents to appropriate Azure DevOps projects.
- Ability to execute remediation actions using Azure Automation runbooks.
- Seamless platform integration with Azure portal, Azure Active Directory, Office365.

### Connectors

- Azure Monitor.
- Azure Advisor.
- Azure Sentinel.

### Plans and Pricing

- Core incident management functionality is available for free!
- Check the “Plans” tab to see premium features.
- All premium features are free with [VIAcode Managed Services](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/viacode_consulting-1089577.viacodems?tab=Overview&flightCodes=viacode).

### Deployment Steps

- Follow [the deployment guide](https://github.com/VIAcode/VIAcode-Incident-Management-System-for-Azure/blob/master/VIAcode%20Incident%20Management%20System%20for%20Azure%20deployment%20and%20%D1%81onfiguration%20guide.md).
- Review available Premium Connectors and install it from Azure Marketplace.

**Visit [viacode.com](https://www.viacode.com) for more information.**

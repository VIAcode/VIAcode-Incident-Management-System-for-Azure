# VIAcode Incident Management System for Azure Release Notes

<!-- TOC -->
- [VIAcode Incident Management System for Azure 1.2.0](#viacode-incident-management-system-for-azure-120)
- [VIAcode Incident Management System for Azure initial](#viacode-incident-management-system-for-azure-initial)  
<!-- TOC END -->

## VIAcode Incident Management System for Azure 1.2.0

### Features

- Integration scenario for Sentinel incidents in VIMS
- Original VIMS KB features
- Fork Zammad and reconfigure pipelines
- VIMS image shall have predefined Group
- VIMS Web UI shall have a button for incident delegation to Azure DevOps
- Add repeat count and substitute custom alert tag with custom field
- Deploy VIMS to VIAcode CSP/ production (Dogfooding)
- Update offer on AMP with all the fixes from this sprint
- Check possibility of manual update of VIMS
- Prepare Dev Demo Env
- Develop arm-template for automate resource creation for alerts
- As CLIENT I want to see Azure Alert status changed to "Acknowledged" to make sure that the incident created in VIMS and set to Open state.
- Upgrade of VIMS on CSP or regular pay as you go subscription to the newer version
- Integrate Azure Dev Ops Connector into VIMS
- Prepare Env for Sprint 2 Demo
- Elaborate hybrid solution for Sentinel Incident
- Update spec with offer details
- Create spec
- Elaborate deployment model for connectors.
- VIMS must have a designated user for Azure Monitor Connector
- New incidents must be created using "Azure Monitor Connector" user + default properties
- VIMS must have a default organizations
- Add state "Delegated" to ticket
- Add state "Pending Review" for ticket
- VIMS should have predefined roles
- Customer can set ticket state to "Pending Review" to return it to Agent, but customer can not close the ticket
- Create a separated offer for VIMS Azure Monitor Connector
- Admin user should have default values for password and login
- Remove "Name Prefix" field from "Settings" page
- Prepare Env for Sprint 3 Demo
- Agent wants to see Delegated view with his delegated tickets
- Change tier of App Service Plan of WebApp to P1V2
- Azure Monitor Connector user must have permissions for Renew AAD functionality
- Azure Monitor Connector should have avatar picture
- Remove direct links to GitHub for js and css files
- Remove GetSLAReport project from VIMS
- Alert's Severity should be mapped to ticket's priority
- Prepare Env for Sprint 4 Demo
- Create missing alerts in VIMS for FarCorner
- "My Assigned Tickets" overview should display delegated and pending review tickets

### Bugfixes

- Incorrect text Welcome to VIAcode
- Remove Z-word from all visible places on staging environment and staging subscription
- Remove "Itsm" Managed App Definition/mainTemplate.json
- VIMS cannot be installed into CSP subscription
- Broken links are displayed in ticket when there are no resource and/or resource group for alert target
- Dates in tickets are in 12-hour format but without specification if it is morning or evening (AM/PM)
- Remove SLA Dashboards from template
- Remove unavailiable locations from UI
- Sometimes function isn't triggered when alert is fired
- All links in emails should be hidden behind the display text.
- Sometimes ticket creation request to Zammad fails with HTTP error 422: Unprocessable Entity when connector function triggers it
- Sometimes two tickets are created for single alert
- Manual ticket creation doesn't work
- BUG incorrect packages on Cloud Partner Portal
- VIMS DB not backed up on upgrade
- Documentation Update TOC  on github
- Customer doesn't receive any notifications
- Fix Azure Monitor connector template
- Z-metric Zammad statistics (version 1.1.15)
- VIMS must have a designated user for Azure DevOps Connector
- Connector role is not localized and doesn't have a description
- Customer receives emails about problem with VIMS administrator account
- Azure DevOps token is visible to user
- Zammad in display strings
- Can not see Delegate button
- Ticket in custom status like delegated is not visible in any view to Agent.
- It is not secure to have password of Azure Monitor Connector hardcoded in GitHub
- Delegate button not always works
- Link to "SearchResults" for AppInsight alert is not working
- Ticket cannot be created if "Threshold"  is not integer value
- Minor bugfixes.

## VIAcode Incident Management System for Azure initial

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

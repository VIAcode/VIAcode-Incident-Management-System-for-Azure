# VIAcode ITSM connector for Azure Release Notes

<!-- TOC -->
- [VIAcode ITSM connector for Azure initial release](#viacode-itsm-connector-for-azure-initial-release)  
<!-- TOC END -->

## VIAcode ITSM connector for Azure initial release

- Creation of tickets with articles in VIAcode Incident Management System for:
    - Azure Monitor alerts
    - Azure Security Center alerts
    - Azure Security Center recommendations
    - Azure Advisor recommendations
    - Budget alerts
- Linking of VIAcode Incident Management System tickets and:
    - The same Azure Monitor alert rule's alerts with the same affected resource
    - Azure Security Center alerts with the same alert name and affected resource
    - Azure Security Center recommendations with the same recommendation name
    - Azure Advisor recommendations with the same recommendation name
- Update of VIAcode Incident Management System ticket on firing/resolution of alert or creation of recommendation to be linked to that ticket
- Changing of states of Azure Monitor and Azure Security Center alerts linked to ticket on ticket (or its ancestor ticket) state change
- Tracking of changes to Azure resources using Azure DevOps Repos and Event Grid Subscription

**Visit [viacode.com](https://www.viacode.com) for more information.**

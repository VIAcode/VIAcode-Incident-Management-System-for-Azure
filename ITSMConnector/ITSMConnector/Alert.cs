//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace ITSMConnector
{
    internal class Constants
    {
        internal const string DateTimeFormat = "MM/dd/yyyy hh:mm:ss";
    }

//https://docs.microsoft.com/en-us/azure/azure-monitor/platform/alerts-common-schema-definitions
    [DataContract]
    public class Alert
    {
        [DataMember(Name = "schemaId")]
        public string SchemaId;
        [DataMember(Name = "data")]
        public Data Data;
    }

    [DataContract(Name = "data")]
    public class Data
    {
        const int LINK_TO_SEARCH_RESULTS_LENGTH_BEFORE_TENANT = 26;

        [DataMember(Name = "essentials")]
        public Essentials Essentials;
        [DataMember(Name = "alertContext")]
        public AlertContext AlertContext;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (Essentials.AlertTargetIDs.Any())
            {
                Essentials.ResourceLink = GetLinkToTarget(Essentials.AlertTargetIDs[0]);
            }
        }

        public string GetLinkToTarget(string targetId)
        {
            string tenant = null;

            if (AlertContext.LinkToSearchResults != null)
                tenant = AlertContext.LinkToSearchResults.Substring(LINK_TO_SEARCH_RESULTS_LENGTH_BEFORE_TENANT, Guid.Empty.ToString().Length);
            else if (AlertContext.Claims != null)
            {
                var tenantIndex = AlertContext.Claims.IndexOf("tenantid\":\"");
                if (tenantIndex != -1)
                    tenant = AlertContext.Claims.Substring(tenantIndex + "tenantid\":\"".Length, Guid.Empty.ToString().Length);
            }

            string linkPrefix;

            if (tenant != null)
                linkPrefix = $"https://portal.azure.com#@{tenant}/resource";
            else
                linkPrefix = "https://portal.azure.com/#resource";

            return $"{linkPrefix}{WebUtility.UrlEncode(targetId)}";
        }
    }

    [DataContract(Name = "essentials")]
    public class Essentials
    {
        [DataMember(Name = "alertId")]
        public string AlertId;
        [DataMember(Name = "alertRule")]
        public string AlertRule;
        [DataMember(Name = "severity")]
        public string severityString;
        public Severity Severity;
        [DataMember(Name = "signalType")]
        public string signalTypeString;
        public SignalType SignalType;
        [DataMember(Name = "monitorCondition")]
        public string monitorConditionString;
        public MonitorCondition MonitorCondition;
        [DataMember(Name = "monitoringService")]
        public string monitoringServiceString;
        public MonitoringService MonitoringService;
        [DataMember(Name = "alertTargetIDs")]
        public List<string> AlertTargetIDs;
        [DataMember(Name = "originAlertId")]
        public string OriginAlertId;
        public DateTime FiredDateTime;
        [DataMember(Name = "firedDateTime")]
        public string firedDateTimeString;
        [DataMember(Name = "description")]
        public string Description;
        [DataMember(Name = "essentialsVersion")]
        public string EssentialsVersion;
        [DataMember(Name = "alertContextVersion")]
        public string AlertContextVersion;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "resourceName")]
        public string ResourceName;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "resourceType")]
        public string ResourceType;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "resourceGroupName")]
        public string ResourceGroupName;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "subscriptionID")]
        public string SubscriptionId;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "resourceLink")]
        public string ResourceLink;

        /// <summary>
        /// this is a synthetic field
        /// </summary>
        [DataMember(Name = "tags")]
        public Dictionary<string, string> Tags;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            FiredDateTime = string.IsNullOrEmpty(firedDateTimeString) ? default(DateTime) : DateTime.Parse(firedDateTimeString);
            firedDateTimeString = FiredDateTime.ToString("MM/dd/yyyy hh:mm:ss");
            if (AlertTargetIDs.Any())
            {
                // / subscriptions / 0c39ec7b - 14d7 - 427f - af50 - 59aab0c0f6fc / resourcegroups / sandbox / providers / microsoft.compute / virtualmachines / cheapvm;
                var cntMoreThanOne = AlertTargetIDs.Count > 1;
                foreach (var alertTargetId in AlertTargetIDs)
                {
                    var split = alertTargetId.Split("/");
                    SubscriptionId += split[2] + (cntMoreThanOne ? "; " : "");
                    if(split.Length < 5) continue;
                    ResourceGroupName += split[4] + (cntMoreThanOne ? "; " : "");
                    if (split.Length < 8) continue;
                    ResourceType += $"{split[7]}/{split[6]}" + (cntMoreThanOne ? "; " : "");
                    if (split.Length < 9) continue;
                    ResourceName += split[8] + (cntMoreThanOne ? "; " : "");
                }
            }
            Enum.TryParse(severityString, out Severity);
            switch (monitoringServiceString)
            {
                case "Platform":
                    MonitoringService = MonitoringService.Platform;
                    break;
                case "Log Analytics":
                    MonitoringService = MonitoringService.LogAnalytics;
                    break;
                case "Application Insights":
                    MonitoringService = MonitoringService.ApplicationInsights;
                    break;
                case "Activity Log - Administrative":
                    MonitoringService = MonitoringService.Administrative;
                    break;
                case "Activity Log - Autoscale":
                    MonitoringService = MonitoringService.Autoscale;
                    break;
                case "Activity Log - Policy":
                    MonitoringService = MonitoringService.Policy;
                    break;
                case "Activity Log - Recommendation":
                    MonitoringService = MonitoringService.Recommendation;
                    break;
                case "Activity Log - Security":
                    MonitoringService = MonitoringService.Security;
                    break;
                case "ServiceHealth":
                    MonitoringService = MonitoringService.ServiceHealth;
                    break;
                case "Resource Health":
                    MonitoringService = MonitoringService.ResourceHealth;
                    break;
                default:
                    break;
            }
            switch (signalTypeString)
            {
                case "Metric":
                    SignalType = SignalType.Metric;
                    break;
                case "Log":
                    SignalType = SignalType.Log;
                    break;
                case "Activity Log":
                    SignalType = SignalType.ActivityLog;
                    break;
                default:
                    break;
            }
            Enum.TryParse(monitorConditionString, out MonitorCondition);
        }
    }

    [DataContract(Name = "alertContext")]
    public class AlertContext
    {
        [DataMember(Name = "properties")]
        public Properties Properties;
        [DataMember(Name = "conditionType")]
        public string ConditionType;
        [DataMember(Name = "condition")]
        public Condition Condition;
        [DataMember(Name = "SearchQuery")]
        public string SearchQuery;
        [DataMember(Name = "SearchIntervalStartTimeUtc")]
        public string searchIntervalStartTimeUtcString;
        public DateTime SearchIntervalStartTimeUtc;
        [DataMember(Name = "SearchIntervalEndtimeUtc")]
        public string searchIntervalEndtimeUtcString;
        public DateTime SearchIntervalEndtimeUtc;
        [DataMember(Name = "ResultCount")]
        public int ResultCount;
        [DataMember(Name = "LinkToSearchResults")]
        public string LinkToSearchResults;
        [DataMember(Name = "SeverityDescription")]
        public string SeverityDescription;
        [DataMember(Name = "WorkspaceId")]
        public string WorkspaceId;
        [DataMember(Name = "SearchIntervalDurationMin")]
        public int SearchIntervalDurationMin;
        [DataMember(Name = "AffectedConfigurationItems")]
        public List<string> AffectedConfigurationItems;
        [DataMember(Name = "SearchIntervalInMinutes")]
        public int SearchIntervalInMinutes;
        [DataMember(Name = "Threshold")]
        public int Threshold;
        [DataMember(Name = "Operator")]
        public string Operator;
        [DataMember(Name = "ApplicationId")]
        public string ApplicationId;
        [DataMember(Name = "SearchResults")]
        public SearchResults SearchResults;
        [DataMember(Name = "IncludedSearchResults")]
        public string IncludedSearchResults;
        [DataMember(Name = "AlertType")]
        public string AlertType;
        [DataMember(Name = "authorization")]
        public Authorization Authorization;
        [DataMember(Name = "channels")]
        public string Channels;
        [DataMember(Name = "claims")]
        public string Claims;
        [DataMember(Name = "caller")]
        public string Caller;
        [DataMember(Name = "correlationId")]
        public string CorrelationId;
        [DataMember(Name = "eventSource")]
        public string EventSource;
        [DataMember(Name = "eventTimestamp")]
        public string eventTimestampString;
        public DateTime EventTimestamp;
        [DataMember(Name = "eventDataId")]
        public string EventDataId;
        [DataMember(Name = "level")]
        public string Level;
        [DataMember(Name = "operationName")]
        public string OperationName;
        [DataMember(Name = "operationId")]
        public string OperationId;
        [DataMember(Name = "status")]
        public string Status;
        [DataMember(Name = "subStatus")]
        public string SubStatus;
        [DataMember(Name = "submissionTimestamp")]
        public string submissionTimestampString;
        public DateTime SubmissionTimestamp;
        [DataMember(Name = "httpRequest")]
        public string HttpRequest;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            SearchIntervalStartTimeUtc = string.IsNullOrEmpty(searchIntervalStartTimeUtcString) ? default(DateTime) : DateTime.Parse(searchIntervalStartTimeUtcString);
            searchIntervalStartTimeUtcString = SearchIntervalStartTimeUtc.ToString(Constants.DateTimeFormat);
            SearchIntervalEndtimeUtc = string.IsNullOrEmpty(searchIntervalEndtimeUtcString) ? default(DateTime) : DateTime.Parse(searchIntervalEndtimeUtcString);
            searchIntervalEndtimeUtcString = SearchIntervalEndtimeUtc.ToString(Constants.DateTimeFormat);
            EventTimestamp = string.IsNullOrEmpty(eventTimestampString) ? default(DateTime) : DateTime.Parse(eventTimestampString);
            eventTimestampString = EventTimestamp.ToString(Constants.DateTimeFormat);
            SubmissionTimestamp = string.IsNullOrEmpty(submissionTimestampString) ? default(DateTime) : DateTime.Parse(submissionTimestampString);
            submissionTimestampString = SubmissionTimestamp.ToString(Constants.DateTimeFormat);
        }
    }

    [DataContract(Name = "properties")]
    public class Properties
    {
        [DataMember(Name = "title")]
        public string Title;
        [DataMember(Name = "details")]
        public string Details;
        [DataMember(Name = "service")]
        public string Service;
        [DataMember(Name = "region")]
        public string Region;
        [DataMember(Name = "currentHealthStatus")]
        public string CurrentHealthStatus;
        [DataMember(Name = "previousHealthStatus")]
        public string PreviousHealthStatus;
        [DataMember(Name = "communication")]
        public string Communication;
        [DataMember(Name = "incidentType")]
        public string IncidentType;
        [DataMember(Name = "trackingId")]
        public string TrackingId;
        [DataMember(Name = "impactStartTime")]
        public string impactStartTimeString;
        public DateTime ImpactStartTime;
        [DataMember(Name = "impactMitigationTime")]
        public string impactMitigationTimeString;
        public DateTime ImpactMitigationTime;
        [DataMember(Name = "impactedServices")]
        public string ImpactedServices;
        [DataMember(Name = "impactedServicesTableRows")]
        public string ImpactedServicesTableRows;
        [DataMember(Name = "defaultLanguageTitle")]
        public string DefaultLanguageTitle;
        [DataMember(Name = "defaultLanguageContent")]
        public string DefaultLanguageContent;
        [DataMember(Name = "stage")]
        public string Stage;
        [DataMember(Name = "type")]
        public string Type;
        [DataMember(Name = "cause")]
        public string Cause;
        [DataMember(Name = "communicationId")]
        public string CommunicationId;
        [DataMember(Name = "maintenanceId")]
        public string MaintenanceId;
        [DataMember(Name = "isHIR")]
        public string IsHIR;
        [DataMember(Name = "version")]
        public string Version;
        [DataMember(Name = "oldInstancesCount")]
        public string OldInstancesCount;
        [DataMember(Name = "newInstancesCount")]
        public string NewInstancesCount;
        [DataMember(Name = "lastScaleActionTime")]
        public string LastScaleActionTime;
        [DataMember(Name = "isComplianceCheck")]
        public string IsComplianceCheck;
        [DataMember(Name = "resourceLocation")]
        public string ResourceLocation;
        [DataMember(Name = "threatStatus")]
        public string ThreatStatus;
        [DataMember(Name = "category")]
        public string Category;
        [DataMember(Name = "filePath")]
        public string FilePath;
        [DataMember(Name = "threatID")]
        public string ThreatID;
        [DataMember(Name = "protectionType")]
        public string ProtectionType;
        [DataMember(Name = "actionTaken")]
        public string ActionTaken;
        [DataMember(Name = "resourceType")]
        public string ResourceType;
        [DataMember(Name = "compromisedEntity")]
        public string CompromisedEntity;
        [DataMember(Name = "remediationSteps")]
        public string RemediationSteps;
        [DataMember(Name = "attackedResourceType")]
        public string AttackedResourceType;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            ImpactStartTime = string.IsNullOrEmpty(impactStartTimeString) ? default(DateTime) : DateTime.Parse(impactStartTimeString);
            impactStartTimeString = ImpactStartTime.ToString(Constants.DateTimeFormat);
            ImpactMitigationTime = string.IsNullOrEmpty(impactMitigationTimeString) ? default(DateTime) : DateTime.Parse(impactMitigationTimeString);
            impactMitigationTimeString = ImpactMitigationTime.ToString(Constants.DateTimeFormat);
        }
    }

    [DataContract(Name = "authorization")]
    public class Authorization
    {
        [DataMember(Name = "action")]
        public string Action;
        [DataMember(Name = "scope")]
        public string Scope;
    }

    [DataContract(Name = "SearchResults")]
    public class SearchResults
    {
        [DataMember(Name = "tables")]
        public List<Tables> Tables;
        [DataMember(Name = "dataSources")]
        public List<DataSources> DataSources;
    }

    [DataContract(Name = "tables")]
    public class Tables
    {
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "columns")]
        public List<Columns> Columns;
        [DataMember(Name = "rows")]
        public List<List<string>> Rows;
    }

    [DataContract(Name = "columns")]
    public class Columns
    {
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "type")]
        public string Type;
    }

    [DataContract(Name = "dataSources")]
    public class DataSources
    {
        [DataMember(Name = "resourceId")]
        public string ResourceId;
        [DataMember(Name = "tables")]
        public List<string> Tables;
    }

    [DataContract(Name = "condition")]
    public class Condition
    {
        [DataMember(Name = "windowSize")]
        public string WindowSize;

        [DataMember(Name = "allOf")]
        public List<AllOf> AllOf;

        [DataMember(Name = "windowStartTime")]
        public string windowStartTimeString;
        public DateTime WindowStartTime;

        [DataMember(Name = "windowEndTime")]
        public string windowEndTimeString;
        public DateTime WindowEndTime;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            WindowStartTime = string.IsNullOrEmpty(windowStartTimeString) ? default(DateTime) : DateTime.Parse(windowStartTimeString);
            windowStartTimeString = WindowStartTime.ToString(Constants.DateTimeFormat);
            WindowEndTime = string.IsNullOrEmpty(windowEndTimeString) ? default(DateTime) : DateTime.Parse(this.windowEndTimeString);
            windowEndTimeString = WindowEndTime.ToString(Constants.DateTimeFormat);
        }
    }

    [DataContract(Name = "allOf")]
    public class AllOf
    {
        [DataMember(Name = "metricName")] public string MetricName;
        [DataMember(Name = "metricNamespace")] public string MetricNamespace;
        [DataMember(Name = "operator")] public string Operator;
        [DataMember(Name = "threshold")] public int Threshold;
        [DataMember(Name = "timeAggregation")] public string TimeAggregation;
        [DataMember(Name = "dimensions")] public List<Dimensions> Dimensions;
        [DataMember(Name = "metricValue")] public decimal MetricValue;
    }

    [DataContract(Name = "dimensions")]
    public class Dimensions
    {
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "value")]
        public string Value;

        public override string ToString() => $"Name: **{Name}** [ Value: **{Value}**]";
    }

    public enum MonitoringService
    {
        Platform,
        LogAnalytics,
        ApplicationInsights,
        Administrative,
        Autoscale,
        Policy,
        Recommendation,
        Security,
        ServiceHealth,
        ResourceHealth,
        Upsell,
        Feed
    }

    public enum Severity
    {
        Sev0,
        Sev1,
        Sev2,
        Sev3,
        Sev4
    }

    public enum SignalType
    {
        Metric,
        Log,
        ActivityLog
    }

    public enum MonitorCondition
    {
        Fired,
        Resolved
    }
}

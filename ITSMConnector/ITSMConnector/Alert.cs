//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITSMConnector
{
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
        [DataMember(Name = "essentials")]
        public Essentials Essentials;
        [DataMember(Name = "alertContext")]
        public AlertContext AlertContext;
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

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            FiredDateTime = string.IsNullOrEmpty(firedDateTimeString) ? default(DateTime) : DateTime.Parse(firedDateTimeString);
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
                    MonitoringService = MonitoringService.ActivityLog;
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
            SearchIntervalEndtimeUtc = string.IsNullOrEmpty(searchIntervalEndtimeUtcString) ? default(DateTime) : DateTime.Parse(searchIntervalEndtimeUtcString);
            EventTimestamp = string.IsNullOrEmpty(eventTimestampString) ? default(DateTime) : DateTime.Parse(eventTimestampString);
            SubmissionTimestamp = string.IsNullOrEmpty(submissionTimestampString) ? default(DateTime) : DateTime.Parse(submissionTimestampString);
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

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            ImpactStartTime = string.IsNullOrEmpty(impactStartTimeString) ? default(DateTime) : DateTime.Parse(impactStartTimeString);
            ImpactMitigationTime = string.IsNullOrEmpty(impactMitigationTimeString) ? default(DateTime) : DateTime.Parse(impactMitigationTimeString);
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
            WindowEndTime = string.IsNullOrEmpty(windowEndTimeString) ? default(DateTime) : DateTime.Parse(this.windowEndTimeString);
        }
    }

    [DataContract(Name = "allOf")]
    public class AllOf
    {
        [DataMember(Name = "metricName")]
        public string MetricName;
        [DataMember(Name = "metricNamespace")]
        public string MetricNamespace;
        [DataMember(Name = "operator")]
        public string Operator;
        [DataMember(Name = "threshold")]
        public int Threshold;
        [DataMember(Name = "timeAggregation")]
        public string TimeAggregation;
        [DataMember(Name = "dimensions")]
        List<Dimensions> Dimensions;
        [DataMember(Name = "metricValue")]
        public decimal MetricValue;
    }

    [DataContract(Name = "dimensions")]
    public class Dimensions
    {
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "value")]
        public string Value;
    }

    public enum MonitoringService
    {
        Platform,
        LogAnalytics,
        ApplicationInsights,
        ActivityLog,
        ServiceHealth,
        ResourceHealth
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

//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using Zammad.Client;
using Zammad.Client.Resources;

namespace ITSMConnector
{
    public class ZammadService
    {
        TicketClient TicketClient;

        // G: 6/15/2008 9:15:07 PM
        private const string DateTimeFormat = "G";
        private const string ZammadUrlSetting = "ZammadUrl";

        public ZammadService()
        {
            string zammadUrl = System.Environment.GetEnvironmentVariable(ZammadUrlSetting);

            var account = ZammadAccount.CreateBasicAccount(zammadUrl, KeyVault.Login, KeyVault.Password);

            TicketClient = account.CreateTicketClient();
        }

        public void CreateTicket(Alert alert)
        {
            TicketArticle ticketArticle = new TicketArticle();
            switch (alert.Data.Essentials.MonitoringService)
            {
                case MonitoringService.Platform:
                    ticketArticle = CreatePlatformArticle(alert.Data.AlertContext);
                    break;
                case MonitoringService.LogAnalytics:
                    ticketArticle = CreateLogAnalyticsArticle(alert.Data.AlertContext);
                    break;
                case MonitoringService.ApplicationInsights:
                    ticketArticle = CreateApplicationInsightsArticle(alert.Data.AlertContext);
                    break;
                case MonitoringService.ActivityLog:
                    ticketArticle = CreateActivityLogArticle(alert.Data.AlertContext);
                    break;
                case MonitoringService.ServiceHealth:
                    ticketArticle = CreateServiceHealthArticle(alert.Data.AlertContext);
                    break;
                case MonitoringService.ResourceHealth:
                    ticketArticle = CreateResourceHealthArticle(alert.Data.AlertContext);
                    break;
                default:
                    ticketArticle = new TicketArticle
                    {
                        Subject = "Default Subject",
                        Body = "Default Body",
                        Type = "note",
                    };
                    break;
            }

            var ticket = TicketClient.CreateTicketAsync(
                new Ticket
                {
                    Title = $"{alert.Data.Essentials.AlertRule}",
                    GroupId = 1,
                    CustomerId = 1,
                    OwnerId = 1

                }, ticketArticle);
        }

        private TicketArticle CreatePlatformArticle(AlertContext context)
        {
            string conditions = $"Window Size: {context.Condition.WindowSize} \n";
            foreach (var metric in context.Condition.AllOf)
            {
                conditions = $"Metric Name : {metric.MetricName} \n " +
                             $"Operator: {metric.Operator} \n " +
                             $"Threshold {metric.Threshold} \n " +
                             $"Time Aggregation: {metric.TimeAggregation}";
                conditions += "\n\n";
            }

            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.ConditionType,
                Body = $"Conditions: \n" +
                       $"{conditions}" +
                       $"Window Start Time: {context.Condition.WindowStartTime.ToString(DateTimeFormat)} \n " +
                       $"Window End Time: {context.Condition.WindowEndTime.ToString(DateTimeFormat)}",
                Type = "note",
            };
            return activityLogArticle;
        }

        private TicketArticle CreateLogAnalyticsArticle(AlertContext context)
        {
            string affectedConfigurationItems = string.Empty;
            foreach (var affectedItem in context.AffectedConfigurationItems)
            {
                affectedConfigurationItems += $"{affectedItem} \n";
            }

            //string tables = FormatSearchResults(context.SearchResults);

            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.AlertType,
                Body = $"Search Query: \n" +
                       $"{context.SearchQuery} \n\n " +

                       $"Link To Search Results: {context.LinkToSearchResults} \n\n" +

                       $"Affected Configuration Items: \n" +
                       $"{affectedConfigurationItems} \n" +

                       $"Search Interval Start Time Utc: {context.SearchIntervalStartTimeUtc.ToString(DateTimeFormat)} \n" +
                       $"Result Count {context.ResultCount} \n " +
                       $"Severity Description: {context.SeverityDescription} \n " +
                       $"Search Interval In Minutes: {context.SearchIntervalInMinutes} \n " +
                       $"Threshold: {context.Threshold} \n " +
                       $"Operator: {context.Operator} \n\n ",

                       //$"Search Result: \n" +
                       //$"{tables}",
                Type = "note"
            };
            return activityLogArticle;
        }

        private TicketArticle CreateApplicationInsightsArticle(AlertContext context)
        {
            //string tables = FormatSearchResults(context.SearchResults);

            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.AlertType,
                Body = $"Search Query {context.SearchQuery} \n " +
                       $"Link To Search Results: {context.LinkToSearchResults} \n" +
                       $"Search Interval Start Time Utc: {context.SearchIntervalStartTimeUtc.ToString(DateTimeFormat)} \n" +
                       $"Result Count {context.ResultCount} \n " +
                       $"Search Interval In Minutes: {context.SearchIntervalInMinutes} \n " +
                       $"Threshold: {context.Threshold} \n " +
                       $"Operator: {context.Operator} \n ",
                       //$"Search Result: {tables}",
                Type = "note"
            };
            return activityLogArticle;
        }

        private TicketArticle CreateActivityLogArticle(AlertContext context)
        {
            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.OperationName,
                Body = $"Authorization: \n" +
                       $"Action: {context.Authorization.Action} \n" +
                       $"Scope: {context.Authorization.Scope} \n\n" +
                       $"Level: {context.Level} \n" +
                       $"Status: {context.Status} \n" +
                       $"Caller: {context.Caller} \n" +
                       $"Event Time: {context.EventTimestamp.ToString(DateTimeFormat)}",
                Type = "note"
            };
            return activityLogArticle;
        }

        private TicketArticle CreateServiceHealthArticle(AlertContext context)
        {
            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.EventSource,
                Body = $"Level: {context.Level} \n " +
                       $"Operation Name: {context.OperationName} \n\n" +

                       $"Properties: \n" +
                       $"Title: {context.Properties.Title} \n" +
                       $"Service: {context.Properties.Service} \n" +
                       $"Region: {context.Properties.Region} \n" +
                       $"ImpactStartTime: {context.Properties.ImpactStartTime.ToString(DateTimeFormat)} \n" +
                       $"Stage: {context.Properties.Stage} \n\n" +

                       $"Status: {context.Status} \n " +
                       $"Event Time: {context.EventTimestamp.ToString(DateTimeFormat)}",
                Type = "note",
                From = context.Caller
            };
            return activityLogArticle;
        }

        private TicketArticle CreateResourceHealthArticle(AlertContext context)
        {
            TicketArticle activityLogArticle = new TicketArticle
            {
                Subject = context.EventSource,
                Body = $"Level: {context.Level} \n " +
                       $"Operation Name: {context.OperationName} \n\n" +

                       $"Properties: \n" +
                       $"Title: {context.Properties.Title} \n" +
                       $"Current Health Status: {context.Properties.CurrentHealthStatus} \n" +
                       $"Previous Health Status: {context.Properties.PreviousHealthStatus} \n" +
                       $"Type: {context.Properties.Type} \n" +
                       $"Cause: {context.Properties.Cause} \n\n" +

                       $"Status: {context.Status} \n " +
                       $"Event Time: {context.EventTimestamp.ToString(DateTimeFormat)}",
                Type = "note",
                From = context.Caller
            };
            return activityLogArticle;
        }

        private string FormatSearchResults(SearchResults searchResults)
        {
            string result = string.Empty;
            foreach (var table in searchResults.Tables)
            {
                result += $"Table Name: {table.Name} \n";
                foreach (var column in table.Columns)
                {
                    result += $"{column.Name} ({column.Type})|";
                }
                result += "\n";
                foreach (var row in table.Rows)
                {
                    foreach (var value in row)
                    {
                        result += $"{value}|";
                    }
                    result += "\n";
                }
                result += "\n\n";
            }
            return result;
        }
    }
}

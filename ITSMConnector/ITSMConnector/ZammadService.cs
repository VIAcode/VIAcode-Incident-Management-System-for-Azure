//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zammad.Client;
using Zammad.Client.Resources;

namespace ITSMConnector
{
    public class ZammadService
    {
        private readonly TicketClient _ticketClient;
        private readonly TagClient _tagClient;

        // G: 6/15/2008 9:15:07 PM
        private const string DateTimeFormat = "G";
        private const string ZammadUrlSetting = "ZammadUrl";

        public ZammadService()
        {
            var zammadUrl = System.Environment.GetEnvironmentVariable(ZammadUrlSetting);

            var account = ZammadAccount.CreateBasicAccount(zammadUrl, KeyVault.Login, KeyVault.Password);

            _ticketClient = account.CreateTicketClient();
            _tagClient = account.CreateTagClient();
        }

        public async Task<Ticket> GetTicketByTitleAsync(string title)
        {
            return (await _ticketClient.SearchTicketAsync($"title:{title} state_id:1", 1, "created_at", "desc")).FirstOrDefault();
        }

        public async Task<IEnumerable<Ticket>> GetClosedTicketsAsync()
        {
            return await _ticketClient.SearchTicketAsync($"state_id:4 close_at:>now-60m", 100, "created_at", "asc");
        }

        public async Task<IEnumerable<string>> GetTicketTagsAsync(int id)
        {
            return await _tagClient.GetTagListAsync("Ticket", id);
        }

        public async Task CreateTicketAsync(string title, string body)
        {
            var ticketArticle = new TicketArticle()
            {
                Subject = title,
                Body = body,
                Type = "note",
            };
            await _ticketClient.CreateTicketAsync(
                new Ticket
                {
                    Title = title,
                    GroupId = 1,
                    CustomerId = 1,
                    OwnerId = 1

                }, ticketArticle);
        }

        public async Task CreateTicketAsync(Alert alert)
        {
            var ticketArticle = MakeTicketArticle(alert);
            var ticket = await _ticketClient.CreateTicketAsync(
                new Ticket
                {
                    Title = $"{alert.Data.Essentials.AlertRule}",
                    GroupId = 1,
                    CustomerId = 1,
                    OwnerId = 1

                }, ticketArticle);

            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"AlertId:{alert.Data.Essentials.AlertId}");
            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"{alert.Data.Essentials.MonitoringService}");
            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"{alert.Data.Essentials.AlertRule}");
        }

        private static TicketArticle MakeTicketArticle(Alert alert)
        {
            TicketArticle ticketArticle;
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

            return ticketArticle;
        }

        public async Task CreateTicketIfNotExistsAsync(Alert alert)
        {
            //If there is an existing open ticket for the rule and the source of the incoming alert,
            //the connector should create a new article for the existing incident.
            var ticket = (await _ticketClient.SearchTicketAsync($"tags:{alert.Data.Essentials.MonitoringService}%20AND%20{alert.Data.Essentials.AlertRule}",
                1)).FirstOrDefault();

            if (ticket == null || ticket.Id == 0 || ticket.CloseAt.HasValue)
            {
                await CreateTicketAsync(alert);
            }
            else
            {
                TicketArticle article;
                if (alert.Data.Essentials.MonitorCondition == MonitorCondition.Resolved)
                {
                    article = new TicketArticle
                    {
                        Subject = alert.Data.AlertContext.AlertType,
                        Body = "Alert resolved",
                        Type = "note"
                    };
                }
                else
                {
                    article = MakeTicketArticle(alert);
                }
                article.TicketId = ticket.Id;

                await _ticketClient.CreateTicketArticleAsync(article);
            }
        }

        private static TicketArticle CreatePlatformArticle(AlertContext context)
        {
            var conditions = $"Window Size: {context.Condition.WindowSize} \n";
            foreach (var metric in context.Condition.AllOf)
            {
                conditions = $"Metric Name : {metric.MetricName} \n " +
                             $"Operator: {metric.Operator} \n " +
                             $"Threshold {metric.Threshold} \n " +
                             $"Time Aggregation: {metric.TimeAggregation}";
                conditions += "\n\n";
            }

            var activityLogArticle = new TicketArticle
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

        private static TicketArticle CreateLogAnalyticsArticle(AlertContext context)
        {
            var affectedConfigurationItems = string.Empty;
            foreach (var affectedItem in context.AffectedConfigurationItems)
            {
                affectedConfigurationItems += $"{affectedItem} \n";
            }

            //string tables = FormatSearchResults(context.SearchResults);

            var activityLogArticle = new TicketArticle
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

        private static TicketArticle CreateApplicationInsightsArticle(AlertContext context)
        {
            //string tables = FormatSearchResults(context.SearchResults);

            var activityLogArticle = new TicketArticle
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

        private static TicketArticle CreateActivityLogArticle(AlertContext context) => new TicketArticle
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

        private static TicketArticle CreateServiceHealthArticle(AlertContext context) => new TicketArticle
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

        private static TicketArticle CreateResourceHealthArticle(AlertContext context)
        {
            var activityLogArticle = new TicketArticle
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

        private static string FormatSearchResult(SearchResults searchResults)
        {
            var result = string.Empty;
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

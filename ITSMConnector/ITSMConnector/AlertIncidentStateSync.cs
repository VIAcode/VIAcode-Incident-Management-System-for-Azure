using System;
using System.Linq;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ITSMConnector
{
    public static class AlertIncidentStateSync
    {
        private static readonly ZammadService ZService = new ZammadService();
        private const string AlertApiVersion = "2019-03-01-preview";
        private const string WarnTitle = "Backward alert state synchronization is not allowed";

        private static readonly string WarnBody = $@"Please provide VIAcode Incident Management with privileges to automatically close Azure alerts when incidents are resolved.
In order to do this, assign the {Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")} Function App a Monitoring Contributor Role for {{0}} subscription in Azure Portal (`Access control (IAM)` blade).
You can also execute the following PS script:
New-AzRoleAssignment -ObjectId (Get-AzADServicePrincipal -SearchString '{Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")}').Id -RoleDefinitionName 'Monitoring Contributor' -Scope '/subscriptions/{{0}}'";

        [FunctionName("AlertIncidentStateSync")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var ticketTask = ZService.GetClosedTicketsAsync();
            ticketTask.Wait();
            var closedTickets = ticketTask.Result;
            foreach (var ticket in closedTickets)
            {
                var tagTask = ZService.GetTicketTagsAsync(ticket.Id);
                tagTask.Wait();
                var alertId = tagTask.Result.Where(r => r.StartsWith("AlertId:")).Select(r => r.Substring(8, (r.Length - 8))).FirstOrDefault();
                CloseAlert(alertId);
            }
        }

        private static void CloseAlert(string alertId)
        {
            string alertSubscription = alertId.Split('/')[2];
            try
            {
                var task = AzureApiClient.UpdateAzResource($"{alertId}/changestate", $"{AlertApiVersion}&newState=Closed", new { });
                task.Wait();
            }
            catch (AggregateException ae) when (ae.InnerExceptions.Any(e => e is WebException))
            {
                var wex = ae.InnerExceptions.OfType<WebException>().First();
                if ((wex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Forbidden)
                {
                    var task = ZService.GetTicketByTitleAsync(WarnTitle);
                    task.Wait();
                    if (task.Result == null)
                        ZService.CreateTicketAsync(WarnTitle, string.Format(WarnBody, alertSubscription)).Wait();
                }
                else if ((wex.Response as HttpWebResponse)?.StatusCode != HttpStatusCode.NotModified)
                    throw;
            }
        }
    }
}

//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System;

namespace ITSMConnector
{
    public static class Function
    {
        private const string AlertApiVersion = "2018-05-05";
        private const string AlertRuleApiVersion = "2018-04-16";
        private const string ActivityLogAlertApiVersion = "2017-04-01";

        private static ILogger Log { get; set; }

        [FunctionName("EntrancePoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Log = log;
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request body: {requestBody}");

            var alert = await ParseAlertAsync(requestBody);

            log.LogInformation($"Alert rule: {alert.Data.Essentials.AlertRule}");
            if(alert.Data.Essentials.Tags != null)
                log.LogInformation($"Alert tags: {string.Join("; ", alert.Data.Essentials.Tags.Select(x => x.Key + "=" + x.Value).ToArray())}");

            log.LogInformation("Start zammad call");
            var zService = new ZammadService();
            await zService.CreateTicketIfNotExistsAsync(alert);
            log.LogInformation("End call");

            return new OkObjectResult("Ok");
        }

        public static Alert ParseAlert(string json)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(typeof(Alert));
            var alert = ser.ReadObject(ms) as Alert;
            ms.Close();
            return alert;
        }

        public static async Task<Alert> ParseAlertAsync(string json)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(typeof(Alert));
            var alert = ser.ReadObject(ms) as Alert;
            ms.Close();

            try
            {
                alert.Data.Essentials.Tags = await GetAlertRuleTagsAsync(alert);
            }
            catch(WebException we)
            {
                Log.LogError(we, $"Connector could not get tags for alert. Response StatusCode: {(we.Response as HttpWebResponse)?.StatusCode}");
            }
            catch(Exception e)
            {
                Log.LogError(e, $"Connector could not get tags for alert. Error message: {e.Message}");
            }
            
            if (string.IsNullOrEmpty(alert.Data.Essentials.Description) && alert.Data.Essentials.SignalType == SignalType.ActivityLog)
            {
                try
                {
                    alert.Data.Essentials.Description = await GetActivityLogAlertDescriptionAsync(alert);
                }
                catch (WebException we)
                {
                    Log.LogError(we, $"Connector could not get description for alert rule. Response StatusCode: {(we.Response as HttpWebResponse)?.StatusCode}");
                }
                catch (Exception e)
                {
                    Log.LogError(e, $"Connector could not get description for alert rule. Error message: {e.Message}");
                }
            }

            return alert;
        }

        public static async Task<Dictionary<string, string>> GetAlertRuleTagsAsync(Alert alert)
        {
            string alertProvider = "microsoft.insights/scheduledQueryRules";
            dynamic alertRuleInfo = await AzureApiClient.GetAzResource(alert.Data.Essentials.SubscriptionId, alert.Data.Essentials.ResourceGroupName, alertProvider, alert.Data.Essentials.AlertRule, AlertRuleApiVersion);
            var tags = alertRuleInfo.tags.ToObject<Dictionary<string, string>>();

            return tags;
        }

        public static async Task<string> GetActivityLogAlertDescriptionAsync(Alert alert)
        {
            string alertProvider = "microsoft.insights/activityLogAlerts";
            dynamic alertRuleInfo = await AzureApiClient.GetAzResource(alert.Data.Essentials.SubscriptionId, alert.Data.Essentials.ResourceGroupName, alertProvider, alert.Data.Essentials.AlertRule, ActivityLogAlertApiVersion);

            var description = alertRuleInfo.properties.description.ToString();
            return description;
        }
    }
}
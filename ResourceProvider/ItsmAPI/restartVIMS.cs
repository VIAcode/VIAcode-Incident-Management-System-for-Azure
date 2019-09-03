using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net;

namespace IncidentRulesAPI
{
    public static class RestartVIMS
    {
        
        [FunctionName("RestartVIMS")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Authorization.AuthorizeRequest(req, log);
            
            var tokenProvider = new AzureServiceTokenProvider();

            var resourceUrl = $"https://management.azure.com{WebUtility.UrlDecode(req.Query["appId"])}/restart?api-version=2016-08-01";
            HttpWebRequest request = WebRequest.CreateHttp(resourceUrl);
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {await tokenProvider.GetAccessTokenAsync("https://management.azure.com/")}");
            request.ContentType = "application/json";
            request.Method = "POST";

            await request.GetResponseAsync();

            return new OkResult();
        }
    }
}
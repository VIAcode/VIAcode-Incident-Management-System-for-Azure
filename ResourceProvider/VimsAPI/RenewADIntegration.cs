using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zammad.Client;
using VIMSConnector;
using Zammad.Client.Resources;
using System.Net;
using System.Text;

namespace IncidentRulesAPI
{
	public static class RenewADIntegration
	{
		private static readonly string vimsUrl = Environment.GetEnvironmentVariable("VimsUrl");
		[FunctionName("RenewADIntegration")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			Authorization.AuthorizeRequest(req, log);

			string name = req.Query["name"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			log.LogInformation(requestBody);

			dynamic data = JsonConvert.DeserializeObject(requestBody);

			var account = ZammadAccount.CreateBasicAccount(vimsUrl, KeyVault.Login, KeyVault.Password);

			HttpWebRequest r = WebRequest.CreateHttp($"{vimsUrl}/api/v1/settings/65");
			r.Method = HttpMethods.Put;
			r.ContentType = "application/json";
			r.Headers.Add(HttpRequestHeader.Authorization, $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{KeyVault.Login}:{KeyVault.Password}"))}");
			using (var stream = r.GetRequestStream())
			using (var writer = new StreamWriter(stream))
				await writer.WriteLineAsync(JsonConvert.SerializeObject(new
				{
					id = 65,
					name = "auth_microsoft_office365_credentials",
					state_current = new
					{
						value = new
						{
							app_id = data.id.Value,
							app_secret = data.secret.Value
						}
					}
				}));
			await r.GetResponseAsync();
			return new OkResult();
		}
	}
}

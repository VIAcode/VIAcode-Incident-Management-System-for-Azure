using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IncidentRulesAPI
{
	internal class Authorization
	{
		internal static void AuthorizeRequest(HttpRequest req, ILogger log)
		{
			log.LogInformation($"Thumbprint: {req.HttpContext.Connection.ClientCertificate.Thumbprint}");
			log.LogInformation($"ProviderPath: {req.Headers["x-ms-customproviders-requestpath"]}");
		}
	}
}
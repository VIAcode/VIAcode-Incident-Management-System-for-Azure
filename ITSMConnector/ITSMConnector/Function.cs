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

namespace ITSMConnector
{
    public static class Function
    {
        [FunctionName("EntrancePoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var alert = ParseAlert(requestBody);

            log.LogInformation($"Body: {requestBody}");

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
    }
}

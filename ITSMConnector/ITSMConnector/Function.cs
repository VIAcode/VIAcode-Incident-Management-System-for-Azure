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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var alert = ParseAlert(requestBody);

            log.LogInformation($"Body: {requestBody}");

            log.LogInformation($"Start zammad call");
            ZammadService zService = new ZammadService();
            zService.CreateTicket(alert);
            log.LogInformation($"End call");

            return (ActionResult)new OkObjectResult("Ok");
        }

        public static Alert ParseAlert(string json)
        {
            var alert = new Alert();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Alert));
            alert = ser.ReadObject(ms) as Alert;
            ms.Close();
            return alert;
        }
    }
}

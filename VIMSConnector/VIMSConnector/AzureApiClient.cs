using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VIMSConnector
{
    public class AzureApiClient
    {
        private const string ApiUrl = "https://management.azure.com/";
        private static readonly AzureServiceTokenProvider TokenProvider = new AzureServiceTokenProvider();

        public static async Task<dynamic> GetAzResource(string resourceId, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}{resourceId}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "GET";
            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> GetAzResource(string subscriptionId, string resourceGroup, string resourceProvider, string id, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{resourceProvider}/{id}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "GET";
            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> GetAzResource(string subscriptionId, string resourceProvider, string id, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/providers/{resourceProvider}/{id}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "GET";
            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> UpdateAzResource(string resourceId, string apiVersion, dynamic properties)
        {
            var aiUrl = $"{ApiUrl}{resourceId}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "POST";
            using (var s = aiRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                await sw.WriteLineAsync(properties.ToString());

            return (await GetResponseAsync(aiRequest)).value;
        }

        public static async Task<dynamic> UpdateAzResource(string subscriptionId, string resourceGroup, string resourceProvider, string id, string apiVersion, dynamic properties)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{resourceProvider}/{id}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "POST";
            using (var s = aiRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                await sw.WriteLineAsync(properties.ToString());

            return (await GetResponseAsync(aiRequest)).value;
        }

        public static async Task<dynamic> UpdateAzResource(string subscriptionId, string resourceProvider, string id, string apiVersion, dynamic properties)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/providers/{resourceProvider}/{id}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "POST";
            using (var s = aiRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                await sw.WriteLineAsync(properties.ToString());

            return (await GetResponseAsync(aiRequest)).value;
        }

        public static async Task<dynamic> ListAzResources(string subscriptionId, string resourceGroup, string resourceProvider, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{resourceProvider}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            return (await GetResponseAsync(aiRequest)).value;
        }

        public static async Task<dynamic> ListAzResources(string subscriptionId, string resourceProvider, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/providers/{resourceProvider}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            return (await GetResponseAsync(aiRequest)).value;
        }

        public static async Task<dynamic> CreateAzResource(string subscriptionId, string resourceGroup, string resourceProvider, string name, dynamic properties, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{resourceProvider}/{name}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "PUT";
            using (var s = aiRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                await sw.WriteLineAsync(properties.ToString());

            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> CreateAzResource(string subscriptionId, string resourceProvider, string name, dynamic properties, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/providers/{resourceProvider}/{name}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "PUT";
            using (var s = aiRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                await sw.WriteLineAsync(properties.ToString());

            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> DeleteAzResource(string resourceId, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}{resourceId}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "DELETE";
            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> DeleteAzResource(string subscriptionId, string resourceGroup, string resourceProvider, string name, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{resourceProvider}/{name}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "DELETE";
            return await GetResponseAsync(aiRequest);
        }

        public static async Task<dynamic> DeleteAzResource(string subscriptionId, string resourceProvider, string name, string apiVersion)
        {
            var aiUrl = $"{ApiUrl}subscriptions/{subscriptionId}/providers/{resourceProvider}/{name}?api-version={apiVersion}";
            var aiRequest = await CreateRequestAsync(aiUrl);
            aiRequest.Method = "DELETE";
            return await GetResponseAsync(aiRequest);
        }

        private static async Task<HttpWebRequest> CreateRequestAsync(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {await TokenProvider.GetAccessTokenAsync(ApiUrl)}");
            request.ContentType = "application/json";
            return request;
        }

        private static async Task<dynamic> GetResponseAsync(WebRequest request)
        {
            try
            {
                using (var response = await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                    return JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
            }
            catch (WebException wex)
            {
#if DEBUG
                if (wex.Response == null) throw;
                using (var stream = wex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                    await reader.ReadToEndAsync();
#endif
                throw;
            }
        }


    }
}

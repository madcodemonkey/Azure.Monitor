using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyticsDirect
{
    public class LogAnalyticsService : ILogAnalyticsService
    {
        private readonly LogAnalyticsSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public LogAnalyticsService(LogAnalyticsSettings settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SendDataAsync(Object someObject)
        {
            string json = JsonConvert.SerializeObject(someObject);
            return await SendDataAsync(json);
        }


        /// <summary>Send a request to the POST API endpoint</summary>
        /// <param name="json">A JSON string</param>
        /// <remarks>Here is the Microsoft example: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/data-collector-api#c-sample </remarks>
        public async Task<bool> SendDataAsync(string json)
        {
            string url = $"https://{_settings.WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

            var dateString = DateTime.UtcNow.ToString("r");
            var signature = BuildSignatureString(json, dateString);

            // This is using Polly behind the scenes (see Startup.cs where it configured).
            HttpClient client = _httpClientFactory.CreateClient("LogAnalytics");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            // LogName is name of the event type that is being submitted to Azure Monitor
            client.DefaultRequestHeaders.Add("Log-Type", _settings.CustomLogName);
            client.DefaultRequestHeaders.Add("Authorization", signature);
            client.DefaultRequestHeaders.Add("x-ms-date", dateString);
            // You can use an optional field to specify the timestamp from the data. If the time field is not specified, Azure Monitor assumes the time is the message ingestion time
            // client.DefaultRequestHeaders.Add("time-generated-field", TimeStampField);

            System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(new Uri(url), httpContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        private string BuildSignatureString(string json, string dateString)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + dateString + "\n/api/logs";
            string hashedString = BuildHashString(stringToHash, _settings.Secret);

            // For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
            string signature = "SharedKey " + _settings.WorkspaceId + ":" + hashedString;
            return signature;
        }

        /// <summary>Build Signature</summary>
        private string BuildHashString(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}

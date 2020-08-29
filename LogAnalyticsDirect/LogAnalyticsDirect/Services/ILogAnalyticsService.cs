using System;
using System.Threading.Tasks;

namespace LogAnalyticsDirect
{
    public interface ILogAnalyticsService
    {
        /// <summary>Serializes and then sends an object as JSON</summary>
        /// <param name="someObject"></param>
        Task<bool> SendDataAsync(Object someObject);

        /// <summary>Send a request to the POST API endpoint</summary>
        /// <param name="json">A JSON string</param>
        /// <remarks>Here is the Microsoft example: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/data-collector-api#c-sample </remarks>
        Task<bool> SendDataAsync(string json);
    }
}
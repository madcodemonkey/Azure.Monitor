using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AppInsightsDirect.Core;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppInsightsDirect
{
    public class AppInsightsFunction
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IDoSomeMath _doSomeMath;

        public AppInsightsFunction(TelemetryClient telemetryClient,IDoSomeMath doSomeMath)
        {
            _telemetryClient = telemetryClient;
            _doSomeMath = doSomeMath;
        }


        [FunctionName("LoggerTests")]
        public IActionResult LoggerTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json#logging
            log.LogDebug("This is a LogDebug message.");
            log.LogDebug("This is {myCount} a LogDebug message using structured logging", 42);

            log.LogInformation("This is a LogInformation message.");
            log.LogInformation("This is {myCount} a LogInformation message using structured logging", 42);

            log.LogWarning("This is a LogWarning message.");
            log.LogWarning("This is {myCount} a LogWarning message using structured logging", 42);

            log.LogError("This is a LogError message.");
            log.LogError("This is {myCount} a LogError message using structured logging", 42);
            log.LogError(new ArgumentException("Outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogError with an exception");

            log.LogCritical("This is a LogCritical message.");
            log.LogCritical("This is {myCount} a LogLogCriticalError message using structured logging", 42);
            log.LogCritical(new ArgumentException("Outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogCritical with an exception");

            return new OkObjectResult("ILogger tests ran");
        }

        [FunctionName("ExceptionTest")]
        public IActionResult ExceptionTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            decimal answer = _doSomeMath.DivideTenByInput(0);
            
            return new OkObjectResult($"Exception test should have failed, but returned {answer} instead!");
        }


        [FunctionName("TelemetryTests")]
        public IActionResult TelemetryTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            _telemetryClient.TrackTrace("This is a TrackTrace with no dictionary!");

            var properties = new Dictionary<string, string>();
            properties.Add("MyDate", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            properties.Add("MyNumber", "34");
            properties.Add("MyJson", "{ \"FirstName\": \"George\", \"LastName\": \"Marshall\"  }");

            _telemetryClient.TrackTrace("This is TrackTrace WITH a dictionary", properties);
            
            _telemetryClient.TrackException(new IndexOutOfRangeException("TrackException Outer index out of range",
                new EndOfStreamException("TrackException Inner stream exception")));
           _telemetryClient.TrackException(new IndexOutOfRangeException("TrackException with Dictionary. Outer index out of range",
                new EndOfStreamException("TrackException with dictionary Inner stream exception")), properties);

            return new OkObjectResult("Telemetry tests ran");
        }

   
    }
}

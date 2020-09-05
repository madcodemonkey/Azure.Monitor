using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

[assembly: FunctionsStartup(typeof(AppInsightsSerilog.Startup))]
namespace AppInsightsSerilog
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(GetLoggingLevelSwitch())
                // .Enrich.With<MyExceptionDetailEnricher>()
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                // .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Events)
                .CreateLogger();

            // Notes
            // Log and LoggerConfiguration         --> requires Serilog
            // CompactJsonFormatter                --> requires Serilog.Formatting.Compact
            // WriteTo.Console                     --> requires Serilog.Sinks.Console
            // WriteTo.ApplicationInsights         --> requires Serilog.Sinks.ApplicationInsights  (see also https://github.com/serilog/serilog-sinks-applicationinsights)
            // .Enrich.WithExceptionDetails()      --> requires Serilog.Exceptions and adds unnecessary additional properties uses over 50+ of your 500 fields. Might be great for file logging, but not Analytics!
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // AddSerilog -- Requires Serilog.Extensions.Logging
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            builder.Services.AddTransient<IDoSomeMath, DoSomeMath>();
        }


        /// <summary>Obtains the log level from configuration or defaults to Verbose</summary>
        private LoggingLevelSwitch GetLoggingLevelSwitch()
        {
            string level = Environment.GetEnvironmentVariable("MinimumLogLevel", EnvironmentVariableTarget.Process) ?? "Verbose";

            try
            {
                var levelEventLevel =  (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level);

                return new LoggingLevelSwitch(levelEventLevel);
            }
            catch
            {
                Console.WriteLine($"Unable to parse log level: {level}");
            }

            return  new LoggingLevelSwitch(LogEventLevel.Verbose);
        }
    }

    
    public class MyExceptionDetailEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception != null)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ExceptionDetail", JsonConvert.SerializeObject(logEvent.Exception)));
            }
        }
    }
}

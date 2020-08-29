using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;

[assembly: FunctionsStartup(typeof(LogAnalyticsDirect.Startup))]
namespace LogAnalyticsDirect
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureLogAnalytics(builder);
        }


        /// <summary>Configure log analytics settings and HttpClient retry logic</summary>
        private void ConfigureLogAnalytics(IFunctionsHostBuilder builder)
        {
            LogAnalyticsSettings settings = new LogAnalyticsSettings
            {
                Secret = Environment.GetEnvironmentVariable("LogAnalyticsSharedKey"),
                CustomLogName = Environment.GetEnvironmentVariable("LogAnalyticsLogName"),
                WorkspaceId = Environment.GetEnvironmentVariable("LogAnalyticsWorkspaceID"),
                UrlStringFormat = Environment.GetEnvironmentVariable("LogAnalyticsUrlFormat")
            };

            builder.Services.AddSingleton(settings);
            builder.Services.AddTransient<ILogAnalyticsService, LogAnalyticsService>();

            builder.Services.AddHttpClient("LogAnalytics", client => { })
                .AddTransientHttpErrorPolicy(bld => bld.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                }));
        }
    }
}

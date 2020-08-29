using AppInsightsDemo.Core;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AppInsightsDemo.Startup))]
namespace AppInsightsDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IDoSomeMath, DoSomeMath>();
        }
    }
}

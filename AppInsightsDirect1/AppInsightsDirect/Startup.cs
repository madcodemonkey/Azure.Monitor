using AppInsightsDirect.Core;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AppInsightsDirect.Startup))]
namespace AppInsightsDirect
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IDoSomeMath, DoSomeMath>();
        }
    }
}

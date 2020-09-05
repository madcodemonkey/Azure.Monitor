# Azure Monitor

Contains Azure Monitor demos.  Here is a break down by project

# AppInsightsDirect
Demostrates an Azure Function using the ILogger and Application Insights NuGet package.

# AppInsightsSerilog
Demostrates an Azure Function using the ILogger, which has been override by Serilog.

# LogAnalyticsDirect
This is not a recommended approach, but demonstrates send raw JSON into the Log Analytics endpoint.
The purpose of this demo is to show:
- The JSON you send is flattened
- If you change the shape of your JSON, things don't get logged.

# LogAnalyticsSerilog
Shows the Serilog sink for Log Analytics in action.
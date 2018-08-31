# Application Insights

Logging to Application Insights can be done using Serilog. It can take up to 5 minutes for logs to show up in Applications Insights.

Quick Steps:
1. Add following nuget packages
```text
Serilog
Serilog.Settings.Configuration
Serilog.Sinks.ApplicationInsights
```

2. Setup logging
```c#
const string INSTRUMENTATION_KEY = "";
var logger = new LoggerConfiguration()
    .WriteTo.ApplicationInsightsTraces(INSTRUMENTATION_KEY)
    .CreateLogger();
```

3. Take advantage of semantic/structure logging
```C#
logger.Information("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Information");
```

4. Application Insights allows you to query semantic/structure information. For instance search for all 'Fatal' errors:
```sql
traces
| where customDimensions.severity == 'Fatal'
```
![Application Insights query](../../media/logging-ai-query.png)



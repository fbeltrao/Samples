using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.AzureAnalytics;
using System;
using System.Threading;

namespace CoreConsoleLogAnalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check here to find out how to get this information from LogAnalytics here https://www.systemcenterautomation.com/2018/05/find-azure-log-analytics-keys/
            const string WORKSPACE_ID = "";
            const string AUTHENTICATION_ID = "";


            var logger = new LoggerConfiguration()
                .WriteTo.AzureAnalytics(WORKSPACE_ID, AUTHENTICATION_ID, new ConfigurationSettings
                {
                    LogName = nameof(CoreConsoleLogAnalytics)
                })
                .CreateLogger();


            logger.Fatal("{Number}: {Application} {severity} log", 1, nameof(CoreConsoleLogAnalytics), "Fatal");
            logger.Error("{Number}: {Application} {severity} log", 1, nameof(CoreConsoleLogAnalytics), "Error");
            logger.Warning("{Number}: {Application} {severity} log", 2, nameof(CoreConsoleLogAnalytics), "Warning");
            logger.Information("{Number}: {Application} {severity} log", 2, nameof(CoreConsoleLogAnalytics), "Information");
            logger.Debug("{Number}: {Application} {severity} log", 3, nameof(CoreConsoleLogAnalytics), "Debug");
            logger.Verbose("{Number}: {Application} {severity} log", 3, nameof(CoreConsoleLogAnalytics), "Verbose");

            logger.Dispose();

            Console.WriteLine("Waiting 10secs for logs to be written");
            Thread.Sleep(1000 * 10);
        }
    }
}

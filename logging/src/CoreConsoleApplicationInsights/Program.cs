using Serilog;
using System;
using System.Threading;

namespace CoreConsoleApplicationInsights
{
    class Program
    {
        static void Main(string[] args)
        {
            const string INSTRUMENTATION_KEY = "";
            var logger = new LoggerConfiguration()
                .WriteTo.ApplicationInsightsTraces(INSTRUMENTATION_KEY)
                .CreateLogger();


            logger.Fatal("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Fatal");
            logger.Error("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Error");
            logger.Warning("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Warning");
            logger.Information("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Information");
            logger.Debug("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Debug");
            logger.Verbose("{Application} {severity} log", nameof(CoreConsoleApplicationInsights), "Verbose");

            logger.Dispose();

            Console.WriteLine("Waiting 10secs for logs to be written");
            Thread.Sleep(1000 * 10);
        }
    }
}

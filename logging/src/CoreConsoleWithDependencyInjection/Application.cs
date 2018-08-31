using Library;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CoreConsoleWithDependencyInjection
{
    public class Application
    {
        static ILogger staticLogger = ApplicationLogging.CreateLogger<Application>();

        private readonly ILogger logger;
        private readonly ICalculator calculator;

        public Application(ILogger<Application> logger, ICalculator calculator)
        {
            this.logger = logger;
            this.calculator = calculator;

            staticLogger.LogDebug($"New {nameof(Application)} created");
        }
        public void Run()
        {
            this.logger.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);


            this.calculator.Sum(1, 2);
            this.calculator.Sum(103, 30);

            staticLogger.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);

            Thread.Sleep(2000);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("PRESS <ENTER> TO EXIT");
                Console.ReadLine();
            }
        }
    }
}

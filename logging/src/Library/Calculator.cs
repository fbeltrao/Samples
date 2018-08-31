using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Calculator : ICalculator
    {
        private readonly ILogger logger;

        public Calculator(ILoggerFactory loggerFactory)
        {
            // create a logging category "calculator"
            this.logger = loggerFactory.CreateLogger("Calculator.Impl");
        }

        public int Sum(int v1, int v2)
        {
            // simulate a bug
            if (v2 == 100 || v1 == 100)
            {
                throw new Exception("Bug when any value is equal to 100");
            }

            var result = v1 + v2;            
            this.logger.LogCritical("Critical: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            this.logger.LogError("Error: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            this.logger.LogWarning("Warning: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            this.logger.LogInformation("Information: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            this.logger.LogDebug("Debug: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            this.logger.LogTrace("Trace: {Operation}, {v1} and {v2}, resulted in {result}", nameof(Sum), v1, v2, result);
            
            return result;
        }

        public int Divide(int v1, int v2)
        {
            if (v2 == 0)
            {
                this.logger.LogError("Division by zero");

                throw new DivideByZeroException();
            }

            var result = v1 / v2;
            this.logger.LogInformation("{Operation}, {v1} and {v2}, resulted in {result}", nameof(Divide), v1, v2, result);
            return result;
        }
    }
}

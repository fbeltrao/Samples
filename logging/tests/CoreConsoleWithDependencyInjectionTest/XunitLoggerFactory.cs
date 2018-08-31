using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace CoreConsoleWithDependencyInjectionTest
{
    public class XunitLoggerFactory : ILoggerFactory
    {
        private readonly ITestOutputHelper output;

        public XunitLoggerFactory(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitLogger(this.output);
        }

        public void Dispose()
        {
        }
    }
}

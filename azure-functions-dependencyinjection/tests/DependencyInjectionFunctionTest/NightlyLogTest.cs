using DependencyInjectionFunction;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DependencyInjectionFunctionTest
{
    public class NightlyLogTest
    {
        [Fact]
        public async Task At_1100_DoesNotLog()
        {
            var collector = new TestAsyncCollector<LogDocument>();
            var result = await NightlyLog.RunAsync("hi", collector, new FixDateTimeResolver(new DateTime(2000, 1, 1, 6, 0, 0, DateTimeKind.Utc)), NullLogger.Instance);
            Assert.Empty(collector);
        }

        [Fact]
        public async Task At_0300_DoesLog()
        {
            var collector = new TestAsyncCollector<LogDocument>();
            var result = await NightlyLog.RunAsync("hi", collector, new FixDateTimeResolver(new DateTime(2000, 1, 1, 3, 0, 0, DateTimeKind.Utc)), NullLogger.Instance);
            Assert.Single(collector);            
            Assert.Equal("hi", collector.First().Log);
        }
    }
}

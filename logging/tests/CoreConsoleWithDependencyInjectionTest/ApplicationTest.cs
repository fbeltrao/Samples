using CoreConsoleWithDependencyInjection;
using Library;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CoreConsoleWithDependencyInjectionTest
{
    public class ApplicationTest
    {
        private readonly ITestOutputHelper output;
        private readonly XunitLoggerFactory factory;
       
        public ApplicationTest(ITestOutputHelper output)
        {
            this.output = output;
            this.factory = new XunitLoggerFactory(output);
            ApplicationLogging.LoggerFactory = this.factory;
        }

        [Fact]
        public void Ctor_Logs_NewInstanceCreated_UsingMock()
        {
            var target = new Application(new XunitLogger<Application>(output), new Mock<ICalculator>().Object);
        }

        [Fact]
        public void Ctor_Logs_NewInstanceCreated_UsingNullLogger()
        {            
            
            var target = new Application(new NullLogger<Application>(), new Mock<ICalculator>().Object);
        }
    }
}

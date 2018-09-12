using DependencyInjectionFunction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using Xunit;

namespace DependencyInjectionFunctionTest
{
    public class PartsOfDayTest
    {
        private readonly HttpRequest httpRequest;

        public PartsOfDayTest()
        {
            this.httpRequest = new Mock<HttpRequest>().Object;
            
        }

        [Fact]
        public void At_0459_ItIsNight()
        {
            var actual = PartsOfDay.Run(this.httpRequest, NullLogger.Instance, new FixDateTimeResolver(new DateTime(2000, 1, 1, 4, 59, 0, DateTimeKind.Utc)));
            Assert.Equal("night", actual);
        }

        [Fact]
        public void At_0500_ItIsMorning()
        {
            var actual = PartsOfDay.Run(this.httpRequest, NullLogger.Instance, new FixDateTimeResolver(new DateTime(2000, 1, 1, 5, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("morning", actual);
        }

        [Fact]
        public void At_1200_ItIsAfternoon()
        {
            var actual = PartsOfDay.Run(this.httpRequest, NullLogger.Instance, new FixDateTimeResolver(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("afternoon", actual);
        }


        [Fact]
        public void At_1700_ItIsEvening()
        {
            var actual = PartsOfDay.Run(this.httpRequest, NullLogger.Instance, new FixDateTimeResolver(new DateTime(2000, 1, 1, 17, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("evening", actual);
        }

        [Fact]
        public void At_2100_ItIsNight()
        {
            var actual = PartsOfDay.Run(this.httpRequest, NullLogger.Instance, new FixDateTimeResolver(new DateTime(2000, 1, 1, 21, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("night", actual);
        }
    }
}

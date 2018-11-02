using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MessageProcessing.Test
{
    public class LoraTelemetryMessageHandlerTest
    {
        ILogger<LoraTelemetryMessageHandler> logger;
        public LoraTelemetryMessageHandlerTest()
        {
            this.logger = NullLogger<LoraTelemetryMessageHandler>.Instance;            
        }

        [Fact]
        public async Task Does_Not_Return_c2d_Message_If_Processed_In_More_Than_3_Seconds()
        {
            var msg = new LoraTelemetryMessage(new byte[10], DateTimeOffset.UtcNow.AddSeconds(-5));
            var target = new LoraTelemetryMessageHandler(this.logger);
            var actual = await target.Handle(msg);            
            Assert.NotNull(actual.Payload);            
            Assert.Equal(0, actual.Payload.Length);
        }

        [Fact]
        public async Task Return_c2d_Message_If_Processed_In_Less_Than_3_Seconds()
        {
            var msg = new LoraTelemetryMessage(new byte[10]);
            var target = new LoraTelemetryMessageHandler(this.logger);
            var actual = await target.Handle(msg);            
            Assert.NotNull(actual.Payload);
            Assert.NotEqual(0, actual.Payload.Length);
        }
    }
}

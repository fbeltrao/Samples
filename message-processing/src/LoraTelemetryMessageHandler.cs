using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MessageProcessing
{
    public class LoraTelemetryMessageHandler : MessageHandler<LoraTelemetryMessage>
    {
        private readonly ILogger logger;

        public LoraTelemetryMessageHandler(ILogger<LoraTelemetryMessageHandler> logger)
        {
            this.logger = logger;
        }

        public override Task<IResponse> Handle(LoraTelemetryMessage message)
        {
            logger.LogInformation("Handling {messageType}", "telemetry");

            LoraTelemetryMessageResponse response = null;

            var ellapsedSinceReceived = DateTimeOffset.UtcNow.Subtract(message.ReceivedTime);
            if (ellapsedSinceReceived > TimeSpan.FromSeconds(2))
            {
                response = new LoraTelemetryMessageResponse();
            }
            else 
            {
                // return with c2d message
                response = new LoraTelemetryMessageResponse(new byte[10]);
            } 

            return Task.FromResult<IResponse>(response);
        }
    }

}
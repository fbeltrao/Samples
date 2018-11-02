using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageProcessing
{

    public class LoraNetworkService : IHostedService
    {
        private readonly ILogger logger;
        private readonly ILoraAntenna loraAntenna;
        private readonly ILoraMessageFactory messageFactory;
        private readonly IMediator mediator;

        public LoraNetworkService(ILogger<LoraNetworkService> logger, ILoraAntenna loraAntenna, ILoraMessageFactory messageFactory, IMediator mediator)
        {
            this.logger = logger;
            this.loraAntenna = loraAntenna;
            this.messageFactory = messageFactory;
            this.mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.loraAntenna.SetReceiver(this.OnLoraPacketReceived);
            await this.loraAntenna.Start();

            this.logger.LogInformation("antenna started");
        }

        private async Task OnLoraPacketReceived(LoraAntennaPacket packet)
        {
            this.logger.LogInformation("Message from antenna received");

             // 1. Find out the message type
            var loraMessage = this.messageFactory.CreateFromPayload(packet);

            // 2. Execute handler
            var response = await this.mediator.Send(loraMessage);

            // 3. Send response if needed
            if (response.Payload.Length > 0)
            {
                await this.loraAntenna.SendMessage(response.Payload);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.FromResult(0);
    }
}
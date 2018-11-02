using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MessageProcessing
{
    public class FakeLoraAntenna : ILoraAntenna
    {
        private readonly ILogger logger;

        public FakeLoraAntenna(ILogger<FakeLoraAntenna> logger)
        {
            this.logger = logger;
        }

        public Task SendMessage(ReadOnlyMemory<byte> msg)
        {
            this.logger.LogInformation("Message received in antenna");
            return Task.FromResult(0);
        }

        Func<LoraAntennaPacket, Task> receiver;
        public void SetReceiver(Func<LoraAntennaPacket, Task> receiver)
        {
            this.receiver = receiver;
        }

        public Task Start()
        {
            Task.Run(this.CreateFakePayloads);
            return Task.FromResult(0);
        }

        private async Task CreateFakePayloads()
        {
            while (true)
            {
                var payload = new LoraAntennaPacket(new byte[10]);

                Task.Run(() => this.receiver(payload));

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    await Task.Delay(1000 * 15);
                }
                else
                {
                    await Task.Delay(1000 * 3);
                }               
            }
        }
    }

}
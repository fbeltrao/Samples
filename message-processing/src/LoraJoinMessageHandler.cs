using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MessageProcessing
{

    public class LoraJoinMessageHandler : MessageHandler<LoraJoinMessage>
    {
        private readonly ILogger logger;

        public LoraJoinMessageHandler(ILogger<LoraJoinMessageHandler> logger)
        {
            this.logger = logger;
        }
        public override Task<IResponse> Handle(LoraJoinMessage message)
        {
            logger.LogInformation("Handling {messageType}", "join");

            return Task.FromResult<IResponse>(new LoraJoinMessageResponse(new byte[10]));
        }
    }
}
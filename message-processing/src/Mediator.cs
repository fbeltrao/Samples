using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageProcessing
{

    public class Mediator : IMediator
    {
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        public Mediator(ILogger<Mediator> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        

        public Task<IResponse> Send(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var messageType = message.GetType();
            try
            {                
                var messageHandlerType = typeof(MessageHandler<>).MakeGenericType(messageType);                
                var msgHandler = (IMessageHandler)this.serviceProvider.GetRequiredService(messageHandlerType);
                return msgHandler.Handle(message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating handler for {messageType}", messageType.Name);
                throw;
            }
        }
    }
}
using System;
using System.Threading.Tasks;

namespace MessageProcessing
{

    public interface IMessage
    {
        ReadOnlyMemory<byte> Payload { get; }
    }

    public interface IResponse
    {
        ReadOnlyMemory<byte> Payload { get; }
    }

    public interface INotification
    {
    }

    public interface IMessageHandler        
    {
        Task<IResponse> Handle(IMessage message);
    }

    public abstract class MessageHandler<TRequest> : IMessageHandler
        where TRequest : IMessage
    {
        async Task<IResponse> IMessageHandler.Handle(IMessage message)
        {
            return await this.Handle((TRequest)message);
        }

        public abstract Task<IResponse> Handle(TRequest request);
    }


    public interface INotificationHandler<T> where T: INotification
    {
        Task Handle(T notification);
    }


}
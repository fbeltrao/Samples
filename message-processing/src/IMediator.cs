using System.Threading.Tasks;

namespace MessageProcessing
{
    public interface IMediator
    {
        Task<IResponse> Send(IMessage request);
        //Task Publish(INotification notification);
    }
}
using System;
using System.Threading.Tasks;

namespace MessageProcessing
{
    public interface ILoraAntenna
    {
        Task Start();

        void SetReceiver(Func<LoraAntennaPacket, Task> receiver);

        Task SendMessage(ReadOnlyMemory<byte> msg);
    }
}
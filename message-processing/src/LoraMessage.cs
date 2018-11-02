using System;

namespace MessageProcessing
{

    public abstract class LoraMessage : IMessage
    {
        public DateTimeOffset ReceivedTime { get; }
        public ReadOnlyMemory<byte> Payload { get; }

        public LoraMessage(ReadOnlyMemory<byte> payload) : this(payload, DateTimeOffset.UtcNow)
        {
        }

        public LoraMessage(ReadOnlyMemory<byte> payload, DateTimeOffset receivedTime)
        {
            this.Payload = payload;
            this.ReceivedTime = receivedTime;
        }
    }


}
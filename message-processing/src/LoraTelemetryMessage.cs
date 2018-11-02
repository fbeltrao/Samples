using System;

namespace MessageProcessing
{
    public class LoraTelemetryMessage : LoraMessage
    {
        public LoraTelemetryMessage(ReadOnlyMemory<byte> payload, DateTimeOffset receivedTime) : base(payload, receivedTime)
        {            
        }
        public LoraTelemetryMessage(ReadOnlyMemory<byte> payload) : base(payload)
        {            
        }
    }


}
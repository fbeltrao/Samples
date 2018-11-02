using System;
using System.Collections.Generic;

namespace MessageProcessing
{
    public class LoraAntennaPacket
    {
        public DateTimeOffset ReceivedDate { get;  }
        public ReadOnlyMemory<byte> Payload { get; }

        public LoraAntennaPacket()
        {
            this.ReceivedDate = DateTimeOffset.UtcNow;    
        }

        public LoraAntennaPacket(byte[] rawPayload)
        {
            this.Payload = new ReadOnlyMemory<byte>(rawPayload);
            this.ReceivedDate = DateTimeOffset.UtcNow;
        }
    }
}
using System;

namespace MessageProcessing
{
    public class LoraJoinMessage : LoraMessage
    {
        public LoraJoinMessage(ReadOnlyMemory<byte> payload, DateTimeOffset receivedTime) : base(payload, receivedTime)
        {            
        }
        public LoraJoinMessage(ReadOnlyMemory<byte> payload) : base(payload)
        {            
        }    }


}
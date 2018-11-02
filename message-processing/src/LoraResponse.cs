using System;

namespace MessageProcessing
{
    public abstract class LoraResponse : IResponse
    {
        public ReadOnlyMemory<byte> Payload { get; }
        
        public LoraResponse()
        {            
        }

        public LoraResponse(ReadOnlyMemory<byte> payload)
        {
            Payload = payload;
        }

        
    }


}
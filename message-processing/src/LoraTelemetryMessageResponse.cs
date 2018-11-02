using System;

namespace MessageProcessing
{
    public class LoraTelemetryMessageResponse : LoraResponse
    {
        public LoraTelemetryMessageResponse()
        {            
        }

        public LoraTelemetryMessageResponse(ReadOnlyMemory<byte> payload) : base(payload)
        {            
        }
    }


}
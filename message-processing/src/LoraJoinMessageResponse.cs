using System;

namespace MessageProcessing
{
    public class LoraJoinMessageResponse : LoraResponse
    {
        public LoraJoinMessageResponse()
        {            
        }

        public LoraJoinMessageResponse(ReadOnlyMemory<byte> payload) : base(payload)
        {            
        }
    }


}
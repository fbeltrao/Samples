using System;

namespace MessageProcessing
{

    public interface ILoraMessageFactory
    {
        LoraMessage CreateFromPayload(LoraAntennaPacket packet);
    }

    public class LoraMessageFactory : ILoraMessageFactory
    {
        Random random = new Random();

        public LoraMessage CreateFromPayload(LoraAntennaPacket packet)
        {
            if (random.Next() % 2 == 0)
                return new LoraJoinMessage(packet.Payload);
                
            return new LoraTelemetryMessage(packet.Payload);
        }
    }
}
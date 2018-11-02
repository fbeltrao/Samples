using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MessageProcessing
{

    public class UdpLoraAntenna : ILoraAntenna
    {
        Func<LoraAntennaPacket, Task> receiver;
        UdpClient udpClient;

        public UdpLoraAntenna()
        {
            
        }

        public void SetReceiver(Func<LoraAntennaPacket, Task> receiver)
        {
            this.receiver = receiver;
        }

        public async Task SendMessage(ReadOnlyMemory<byte> payload)
        {
            await udpClient.SendAsync(payload.ToArray(), payload.Length);
        }

        public async Task Start()
        {
            const int PORT = 1000;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            udpClient = new UdpClient(endPoint);


            while (true)
            {
                UdpReceiveResult receivedResults = await udpClient.ReceiveAsync();

                var packet = new LoraAntennaPacket(receivedResults.Buffer);

                // do not wait for it to finish
                Task.Run(()=> this.receiver(packet));
            }        
        }
    }
}
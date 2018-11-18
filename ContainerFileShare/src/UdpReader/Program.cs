using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //IPEndPoint localpt = new IPEndPoint(IPAddress.Parse("192.168.1.24"), 6000);
                //IPEndPoint localpt = new IPEndPoint(IPAddress.Broadcast, 6000);
                //IPEndPoint localpt = new IPEndPoint(IPAddress.Loopback, 6000);
                IPEndPoint localpt = new IPEndPoint(IPAddress.Any, 6000);
                
                UdpClient udpServer = new UdpClient(localpt);

                // UdpClient udpServer = new UdpClient();
                // udpServer.ExclusiveAddressUse = false;
                // udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                // udpServer.Client.Bind(localpt);
                // Console.WriteLine("Listening on " + localpt + ".");
                while (true)
                {
                    //IPEndPoint inEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    
                    var result = await udpServer.ReceiveAsync();
                    //byte[] buffer = udpServer.Receive(ref inEndPoint);        
                    //if (buffer != null && buffer.Length > 0)
                    //var result = await udpClient.ReceiveAsync();
                    if (result != null && result.Buffer != null && result.Buffer.Length > 0)
                    {
                        var text = Encoding.UTF8.GetString(result.Buffer);
                        Console.WriteLine($"[Read] {text} from {result.RemoteEndPoint.ToString()}");
                    }
                    else
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.ExitCode = 1;
            }
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpWriter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                int delayInMs = 1000;
                IPEndPoint localpt = new IPEndPoint(IPAddress.Broadcast, 6000);
                if (args.Length > 0)
                {
                    localpt = new IPEndPoint(IPAddress.Parse(args[0]), 6000);

                    if (args.Length > 1)
                    {
                        if (Int32.TryParse(args[1], out var delayInMsArg) && delayInMsArg > 0)
                        {
                            delayInMs = delayInMsArg;
                        }
                    }
                }
                
                Console.WriteLine("Will send to " + localpt.ToString());
                
                var udpClient = new UdpClient();
                udpClient.ExclusiveAddressUse = false;
                
              
                long count = 0;
                while (true)
                {
                    var text = $"[{DateTime.UtcNow.ToShortTimeString()}] {count++}";
                    Console.WriteLine("Wrote:" + text);
                    var textInBytes = Encoding.UTF8.GetBytes(text);
                    udpClient.Send(textInBytes, textInBytes.Length, localpt);
                    
                    await Task.Delay(delayInMs);
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

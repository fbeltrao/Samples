namespace TransparentGatewayModule
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Shared;

    class Program
    {
        static void Main(string[] args)
        {
            Init().Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {
            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            _ = StartDeviceClientSimulator();

        }

        private static async Task StartDeviceClientSimulator()
        {
            // Create connection on behalf of a device
            var messageId = 1;
            while (true)
            {
                var usingGateway = false;
                var connectionString = "HostName=<iothub>.azure-devices.net;DeviceId=iotedgehubdev4test2;SharedAccessKey=<key>";
                if ((messageId / 3) % 2 == 0)
                {
                    connectionString += ";GatewayHostName=localhost";
                    usingGateway = true;
                }

                using (var deviceClient = DeviceClient.CreateFromConnectionString(connectionString))
                {
                    Console.WriteLine($"[Gateway: {usingGateway}] Device client created");

                    // 1. Get twin, wait 1 second
                    var twins = await deviceClient.GetTwinAsync();
                    var counter = 0;
                    if (twins.Properties.Reported.Contains("counter"))
                    {
                        counter = (int)twins.Properties.Reported["counter"];
                    }
                    Console.WriteLine($"[Gateway: {usingGateway}] Twin properties read, counter: {counter}");
                    await Task.Delay(1000);


                    // 2. Updated twin, wait 1 second
                    counter++;
                    var updatedTwin = new TwinCollection();
                    updatedTwin["counter"] = counter;
                    await deviceClient.UpdateReportedPropertiesAsync(updatedTwin);
                    Console.WriteLine($"[Gateway: {usingGateway}] Reported properties updated, counter: {counter}");
                    await Task.Delay(1000);


                    // 3. Check for cloud message
                    var cloudMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));
                    if (cloudMessage != null)
                    {
                        Console.WriteLine($"[Gateway: {usingGateway}] Cloud message received: {Encoding.UTF8.GetString(cloudMessage.GetBytes())}");
                        await deviceClient.CompleteAsync(cloudMessage);
                        Console.WriteLine($"[Gateway: {usingGateway}] Cloud message completed");
                    }
                    await Task.Delay(1000);


                    // 4. Send event 
                    await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(messageId.ToString())));
                    Console.WriteLine($"[Gateway: {usingGateway}] Sent event {messageId}"); 
                    await Task.Delay(1000);

                    // wait 3 seconds
                    await Task.Delay(3 * 1000);

                    messageId++;
                }  
            }
        }
    }
}

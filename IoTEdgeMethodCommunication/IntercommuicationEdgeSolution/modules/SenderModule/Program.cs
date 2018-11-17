namespace SenderModule
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
    using System.Diagnostics;
    using Newtonsoft.Json;

    class Program
    {
        static int counter;
        // static Microsoft.Azure.Devices.ServiceClient serviceClient;

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

            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");
            Console.WriteLine($"Using connection string: {connectionString ?? "n/a"}");

            // try
            // {
            //     serviceClient = Microsoft.Azure.Devices.ServiceClient.CreateFromConnectionString(connectionString);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Error creating service client: {ex.ToString()}");
            // }

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", PipeMessage, ioTHubModuleClient);
        }


        // static async Task<string> CallModuleUsingServiceClient(string deviceId, string libraryModule)
        // {
        //     if (serviceClient != null)
        //     {
        //         Console.WriteLine($"Will call module method: {deviceId}, {libraryModule}");

        //         var methodRequest = new Microsoft.Azure.Devices.CloudToDeviceMethod("test");
        //         methodRequest.SetPayloadJson(JsonConvert.SerializeObject(new { payload= "ABCDEF0987654321", fport=1 }));

        //         var stopwatch = Stopwatch.StartNew();
                
        //         var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, libraryModule, methodRequest);
        //         stopwatch.Stop();

        //         Console.WriteLine($"Called module function: {response.ToString()} in {stopwatch.ElapsedMilliseconds}ms");
        //         return response.ToString();
        //     }
        //     else
        //     {
        //         Console.WriteLine($"Not calling module {libraryModule} because serviceClient is null");
        //         return "n/a";
        //     }
        // }

        static Random random = new Random();
        static async Task<string> CallModuleUsingModuleClient(string deviceId, string libraryModule, ModuleClient client)
        {                
                Console.WriteLine($"Will call module method: {deviceId}, {libraryModule}");

                var payloadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { payload= "ABCDEF0987654321", fport=1 }));
                var methodName = random.Next(2) % 2 == 0 ? "test1" : "test2";
                var methodRequest = new MethodRequest(methodName, payloadData);
                
                var stopwatch = Stopwatch.StartNew();
                var response = await client.InvokeMethodAsync(deviceId, libraryModule, methodRequest);
                stopwatch.Stop();

                var responseText = Encoding.UTF8.GetString(response.Result);
                Console.WriteLine($"Called module function: {responseText} in {stopwatch.ElapsedMilliseconds}ms");
                return responseText;
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            try
            {
                int counterValue = Interlocked.Increment(ref counter);

                var moduleClient = userContext as ModuleClient;
                if (moduleClient == null)
                {
                    throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
                }

                byte[] messageBytes = message.GetBytes();
                string messageString = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");
                
                var libraryModule = "LibraryModule";
                var deviceId = System.Environment.GetEnvironmentVariable("IOTEDGE_DEVICEID");
                // var resultFromServiceClient = await CallModuleUsingServiceClient(deviceId, libraryModule);
                var resultFromModuleClient = await CallModuleUsingModuleClient(deviceId, libraryModule, moduleClient);
                       
                if (!string.IsNullOrEmpty(messageString))
                {
                    var pipeMessage = new Microsoft.Azure.Devices.Client.Message(messageBytes);
                    foreach (var prop in message.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }

                    // pipeMessage.Properties.Add("resultFromServiceClient", resultFromServiceClient);
                    pipeMessage.Properties.Add("resultFromModuleClient", resultFromModuleClient);

                    await moduleClient.SendEventAsync("output1", pipeMessage);
                    Console.WriteLine("Received message sent");
                }
                

                return MessageResponse.Completed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return MessageResponse.None;
            }
        }
        
    }
}

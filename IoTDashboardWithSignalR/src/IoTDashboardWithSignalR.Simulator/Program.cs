using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDashboardWithSignalR.Simulator
{
    class Program
    {
        static Random random = new Random();


        static async Task<IList<string>> CreateDevices(string connectionString, int devicesCount = 10)
        {
            var list = new List<string>();

            var registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            var devices = new List<Device>();
            for (int i=0; i < devicesCount; ++i)
            {
                var deviceId = $"device{i.ToString("000")}";
                devices.Add(new Device(deviceId));
                list.Add(deviceId);
            }

            var result = await registryManager.AddDevices2Async(devices);
            if (!result.IsSuccessful)
                return new string[0];

            return list;
        }


        static async System.Threading.Tasks.Task<IList<string>> GetAllDevices(string connectionString)
        {
            var list = new List<string>();

            var registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            var query = registryManager.CreateQuery("SELECT * FROM devices", 200);
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                foreach (var twin in page)
                {
                    // do work on twin object
                    if (twin.Status == DeviceStatus.Enabled)
                        list.Add(twin.DeviceId);
                }
            }

            return list;

        }

        static async System.Threading.Tasks.Task<int> Main(string[] args)
        {
            var connectionString = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("iothub");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("IoT Hub connection string not found");
                return 1;
            }
            
            var devices = await GetAllDevices(connectionString);
            if (devices.Count == 0)
                devices = await CreateDevices(connectionString);

            if (devices.Count == 0)
            {
                Console.WriteLine("No devices were found/created.");
                return 1;
            }


            var exiting = new SemaphoreSlim(1, 1);
            await exiting.WaitAsync();

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exiting.Release();
            };


            var cts = new CancellationTokenSource();

            foreach (var device in devices)
            {
                Task.Factory.StartNew(() => SimulateDevice(connectionString, device, cts));
            }

            // Will wait here until finished
            await exiting.WaitAsync();

            cts.Cancel();

            await Task.Delay(3000);

            return 0;
        }


        private static void SimulateDevice(string connectionString, string deviceId, CancellationTokenSource cts)
        {
            try
            {
                var deviceClient = DeviceClient.CreateFromConnectionString(
                    connectionString,
                    deviceId,
                    new ITransportSettings[]
                    {
                        new AmqpTransportSettings(Microsoft.Azure.Devices.Client.TransportType.Amqp_Tcp_Only)
                        {
                            AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings()
                            {
                                Pooling = true
                            }
                        }
                    });

                deviceClient.OpenAsync().GetAwaiter();


                // start with a random delay to no send items in same block
                Thread.Sleep(random.Next(1000));

                var temperature = 10d + random.Next(10) + random.NextDouble();
                var humidity = 40d + random.Next(20) + random.NextDouble();

                var deviceNumber = int.Parse(new string(deviceId.Where(c => char.IsDigit(c)).ToArray()));
                var building = $"building_{(deviceNumber % 3) + 1}";

                while (!cts.IsCancellationRequested)
                {
                    var payload = new
                    {
                        deviceId = deviceId,
                        building = building,
                        temperature = temperature,
                        humidity = humidity,
                        time = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds

                    };

                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    deviceClient.SendEventAsync(new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(jsonPayload))
                    {
                        ContentEncoding = "UTF-8",
                        ContentType = "application/json"
                    }).GetAwaiter().GetResult();

                    Console.WriteLine($"{deviceId}: {jsonPayload}");

                    Thread.Sleep(1000 * 5);

                    temperature = NextMeasurement(temperature, 50, 2, minValue: -10, maxValue: 40);
                    humidity = NextMeasurement(humidity, 50, 1, minValue: 10, maxValue: 80);

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in device {deviceId}: {ex.ToString()}");
            }
        }

        private static double NextMeasurement(double current, int changeRatePercentage, double variance, double minValue, double maxValue)
        {
            // need change?
            if (random.Next(100) >= changeRatePercentage)
                return current;

            // variance = 5
            // min = -5
            var min = -variance;

            // varianceWidth = (5 * 100) - (-5 * 100) => 1000
            var varianceWidth = (variance * 100) - (min * 100);
            
            // actualVariance = random(0-999) / 100 => range 0.00 - 9.99
            var actualVariance = random.Next((int)varianceWidth) / 100;
            var newValue = current + (min + actualVariance);
            newValue = Math.Min(maxValue, Math.Max(minValue, newValue));
            return newValue;
        }
    }
}

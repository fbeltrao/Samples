using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();
            configurationRoot.GetValue<Configuration>("config");
            var config = configurationRoot.Get<Configuration>();

            var cts = new CancellationTokenSource();
            try
            {
                var devices = new IoTDeviceManager(cts.Token);
                devices.GetDevice("3", config.Device3ConnectionString).Post(new UpstreamDeviceMessage("hello world"));


                await Task.Delay(1000);

                devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceMessage("first_one"));          
                var t = devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceMessage("to_wait"));
                Console.WriteLine("Waiting for message 'to_wait'");
                await t.WaitAsync();
                Console.WriteLine("Done waiting for message 'to_wait'");
                devices.GetDevice("1", config.Device1ConnectionString).Post(new UpdateDesiredPropertiesDeviceMessage(new TwinCollection("{ \"myValue\": 1 }")));

                for (var i=0; i < 20; i++)
                {
                    devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceMessage(i.ToString()));
                    devices.GetDevice("2", config.Device2ConnectionString).Post(new UpstreamDeviceMessage(i.ToString()));
                }

                await Task.WhenAll(
                    devices.GetDevice("1", config.Device1ConnectionString).WaitAsync(),
                    devices.GetDevice("2", config.Device2ConnectionString).WaitAsync(),
                    devices.GetDevice("3", config.Device3ConnectionString).WaitAsync()
                    );

                cts.Cancel();
                // this will wait for all pendings tasks to be finished
                Console.WriteLine("Waiting for devices to be finished");
                //cts.Cancel(throwOnFirstException: false);
                await devices.WaitFinished();

                Console.WriteLine("Done waiting for all devices");
                //await Task.WhenAll(device1.WaitAsync(), device2.WaitAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}

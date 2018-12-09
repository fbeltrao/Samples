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
                var strategy = new DefaultDeviceCleanupStrategy(TimeSpan.FromSeconds(config.DisconnectDeviceTimeoutInSeconds), TimeSpan.FromSeconds(config.RemoveDeviceTimeoutInSeconds));
                var devices = new IoTDeviceManager(strategy, cts.Token);
                devices.GetDevice("3", config.Device3ConnectionString).Post(new UpstreamDeviceAction("hello world"));


                await Task.Delay(1000 * 15);

                devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceAction("first_one"));          
                var t = devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceAction("to_wait"));
                Console.WriteLine("Waiting for message 'to_wait'");
                await t.WaitAsync();
                Console.WriteLine("Done waiting for message 'to_wait'");
                devices.GetDevice("1", config.Device1ConnectionString).Post(new UpdateDesiredPropertiesDeviceAction(new TwinCollection("{ \"myValue\": 1 }")));

                for (var i=0; i < 20; i++)
                {
                    devices.GetDevice("1", config.Device1ConnectionString).Post(new UpstreamDeviceAction(i.ToString()));
                    devices.GetDevice("2", config.Device2ConnectionString).Post(new UpstreamDeviceAction(i.ToString()));
                    await Task.Delay(10);
                }

                var higherPriorityTask = devices.GetDevice("1", config.Device1ConnectionString).PriorityPost(new UpstreamDeviceAction("higher_priority"));
                Console.WriteLine("Waiting for higher_priority");
                await higherPriorityTask.WaitAsync();
                Console.WriteLine("Done waiting for higher_priority");


                var c2dMessage = (GetCloudToDeviceMessageAction)devices.GetDevice("1", config.Device1ConnectionString).PriorityPost(new GetCloudToDeviceMessageAction());
                await c2dMessage.WaitAsync();
                Console.WriteLine($"Found c2d message: {c2dMessage.Result != null}");


                await Task.Delay(1000 * 40);

                await Task.WhenAll(
                    devices.GetDevice("1", config.Device1ConnectionString).WaitAsync(),
                    devices.GetDevice("2", config.Device2ConnectionString).WaitAsync(),
                    devices.GetDevice("3", config.Device3ConnectionString).WaitAsync()
                    );



                devices.GetDevice("3", config.Device3ConnectionString).Post(new UpstreamDeviceAction("last one, should reconnect"));


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

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    class IoTDeviceManager
    {
        private readonly CancellationToken cts;
        private readonly AsyncManualResetEvent cleanupThreadFinished;
        ConcurrentDictionary<string, IoTDeviceActor> devices = new ConcurrentDictionary<string, IoTDeviceActor>();
        private readonly Thread cleanupThread;
        

        public IoTDeviceManager(CancellationToken cts)
        {
            this.cts = cts;

            this.cleanupThreadFinished = new AsyncManualResetEvent();
            this.cleanupThread = new Thread(CleanUp)
            {
                // This is important as it allows the process to exit while this thread is running
                IsBackground = true
            };
            cleanupThread.Start();
           
        }

        private void CleanUp()
        {
            Console.WriteLine("Starting device cleanup task");
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    if (!cts.IsCancellationRequested)
                    {
                        foreach (var kv in devices)
                        {
                            if (!kv.Value.HasWork())
                            {
                                if (devices.TryRemove(kv.Key, out _))
                                {
                                    Console.WriteLine($"[{kv.Key}] removed");
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Ending device cleanup task");
            cleanupThreadFinished.Set();
        }

        public IoTDeviceActor GetDevice(string id, string connString)
        {
            return this.devices.GetOrAdd(id, (deviceId) => new IoTDeviceActor(id, connString));
        }

        public async Task WaitFinished()
        {
            await this.cleanupThreadFinished.WaitAsync();
        }
    }
}

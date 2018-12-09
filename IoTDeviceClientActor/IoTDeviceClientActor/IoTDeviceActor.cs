using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class IoTDeviceActor
    {
        internal DeviceClient deviceClient;

        public IoTDeviceActor(string id, string connectionString)
        {
            var transportSettings = new ITransportSettings[]
            {
                new AmqpTransportSettings(TransportType.Amqp_Tcp_Only)
                {
                    AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings()
                    {
                        Pooling = true,
                    }
                },
            };

            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, transportSettings);
            Id = id;
        }

        Queue<DeviceMessage> tasks = new Queue<DeviceMessage>();
        volatile DeviceMessage runningTask;

        public string Id { get; }

        public DeviceMessage Post(DeviceMessage msg)
        {
            lock (this)
            {
                if (this.runningTask == null)
                {
                    StartNextTask(msg);
                }
                else
                {
                    this.tasks.Enqueue(msg);
                }
            }

            return msg;
            
        }

        public bool HasWork()
        {
            lock (this)
            {
                return this.tasks.Count > 0 || this.runningTask != null;
            }
        }

        private void StartNextTask(DeviceMessage msg)
        {
            this.runningTask = msg;
            msg.Run(this)
                .ContinueWith(OnTaskCompleted)
                .ConfigureAwait(false);
        }

        private void OnTaskCompleted(Task task)
        {
            lock (this)
            {
                this.runningTask = null;
                if (this.tasks.TryDequeue(out var nextTask))
                {
                    StartNextTask(nextTask);
                }
            }
        }

        public async Task WaitAsync()
        {
            while (true)
            {
                lock (this)
                {
                    if (this.runningTask == null && this.tasks.Count == 0)
                        return;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }

        }
    }
}

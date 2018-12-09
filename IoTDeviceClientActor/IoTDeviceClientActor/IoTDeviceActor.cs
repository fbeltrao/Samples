using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public sealed class IoTDeviceActor : IDisposable
    {
        internal DeviceClient deviceClient;
        

        public IoTDeviceActor(string id, string connectionString)
        {
            Id = id;
            this.connectionString = connectionString;
            this.lastActivityUtc = DateTime.UtcNow;
        }

        readonly Queue<DeviceAction> tasks = new Queue<DeviceAction>();
        readonly Queue<DeviceAction> priorityTasks = new Queue<DeviceAction>();
        volatile DeviceAction runningTask;

        private DateTime lastActivityUtc;
        public DateTime LastActivityUtc { get { return this.lastActivityUtc; } }

        public string Id { get; }

        private string connectionString;

        public DeviceAction Post(DeviceAction action) => InternalPost(this.tasks, action);

        public DeviceAction PriorityPost(DeviceAction action) => InternalPost(this.priorityTasks, action);

        DeviceAction InternalPost(Queue<DeviceAction> queue, DeviceAction action)
        {
            lock (this)
            {
                if (this.runningTask == null)
                {
                    StartNextTask(action);
                }
                else
                {
                    queue.Enqueue(action);
                }

                this.lastActivityUtc = DateTime.UtcNow;
            }

            return action;
        }

        internal bool IsConnected() => this.deviceClient != null;


        internal void Disconnect()
        {
            this.deviceClient?.Dispose();
            this.deviceClient = null;
        }

        public bool HasWork()
        {
            return this.tasks.Count > 0 || this.priorityTasks.Count > 0 || this.runningTask != null;
        }

        private void StartNextTask(DeviceAction msg)
        {
            this.runningTask = msg;
      
            this.RunAction(msg)
                .ContinueWith(OnTaskCompleted, TaskContinuationOptions.ExecuteSynchronously) // TODO: verify if it is better
                .ConfigureAwait(false);
        }

        private void OnTaskCompleted(Task task)
        {
            lock (this)
            {
                this.runningTask = null;
                this.lastActivityUtc = DateTime.UtcNow;
                (bool found, DeviceAction nextAction) = TryDequeueNextTask();
                if (found)
                {
                    StartNextTask(nextAction);
                }
            }
        }

        (bool found, DeviceAction action) TryDequeueNextTask()
        {
            if (this.priorityTasks.TryDequeue(out var task))
                return (true, task);

            if (this.tasks.TryDequeue(out task))
                return (true, task);

            return (false, null);
        }

        private async Task RunAction(DeviceAction msg)
        {
            try
            {
                switch (msg)
                {
                    case UpstreamDeviceAction upstreamAction:
                        {
                            await SafeDeviceClient().SendEventAsync(new Message(Encoding.UTF8.GetBytes(upstreamAction.Body)));
                            Console.WriteLine($"[{Id}] Sent: {upstreamAction.Body}");
                            break;
                        }

                    case UpdateDesiredPropertiesDeviceAction updateDesiredPropertiesAction:
                        {
                            await SafeDeviceClient().UpdateReportedPropertiesAsync(updateDesiredPropertiesAction.ReportedProperties);
                            Console.WriteLine($"[{Id}] Updated reported properties");
                            break;

                        }

                    case GetCloudToDeviceMessageAction getCloudToDeviceMessageAction:
                        {
                            getCloudToDeviceMessageAction.Result = await SafeDeviceClient().ReceiveAsync(getCloudToDeviceMessageAction.Timeout);
                            Console.WriteLine($"[{Id}] Received message");
                            break;
                        }

                    case CompleteCloudToDeviceMessageAction completeCloudToDeviceMessage:
                        {
                            await SafeDeviceClient().CompleteAsync(completeCloudToDeviceMessage.Message);
                            Console.WriteLine($"[{Id}] Completed message");
                            break;
                        }

                    case AbandonCloudToDeviceMessageAction abandonCloudToDeviceMessage:
                        {
                            await SafeDeviceClient().AbandonAsync(abandonCloudToDeviceMessage.Message);
                            Console.WriteLine($"[{Id}] Abandoned message");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                // TODO: add error handling
            }

            msg.Complete();
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

        public void Dispose()
        {
            this.deviceClient?.Dispose();
            this.deviceClient = null;
            GC.SuppressFinalize(this);
        }

        DeviceClient SafeDeviceClient()
        {
            if (this.deviceClient == null)
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

                this.deviceClient = DeviceClient.CreateFromConnectionString(connectionString, transportSettings);
            }

            return this.deviceClient;
        }
    }
}

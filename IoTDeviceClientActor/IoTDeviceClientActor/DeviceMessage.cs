using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public abstract class DeviceMessage
    {
        AsyncManualResetEvent jobDone = new AsyncManualResetEvent();
        public async Task Run(IoTDeviceActor device)
        {
            try
            {
                await this.Execute(device);
            }
            finally
            {
                this.jobDone.Set();
            }
        }

        public async Task WaitAsync() => await this.jobDone.WaitAsync();


        protected abstract Task Execute(IoTDeviceActor device);
    }
}

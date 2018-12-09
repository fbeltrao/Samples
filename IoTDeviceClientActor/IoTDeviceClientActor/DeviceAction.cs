using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public abstract class DeviceAction
    {
        AsyncManualResetEvent jobDone = new AsyncManualResetEvent();

        public void Complete() => this.jobDone.Set();

        public async Task WaitAsync() => await this.jobDone.WaitAsync();
    }
}

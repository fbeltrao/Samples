using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class AsyncManualResetEvent
    {
      
        private volatile TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();

        public Task WaitAsync() { return m_tcs.Task; }

        public void Set() { m_tcs.TrySetResult(true); } 
    }
}

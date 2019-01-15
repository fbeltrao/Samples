using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using System.Net.Http;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace iothubforwarder
{
    public static class TelemetryToAppFunction
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName(nameof(TelemetryToApp))]
        public static async Task TelemetryToApp([IoTHubTrigger ("messages/events", Connection = "EventHubConnectionString", ConsumerGroup = "<your-consumer-group>")] EventData[] messages, ILogger log)
        {
            foreach (var eventData in messages)
            {
                // Ignore twin updates
                if (eventData.Properties.ContainsKey("iothub-message-schema"))
                    continue;
                    
                var deviceId = eventData.SystemProperties["iothub-connection-device-id"].ToString();
                var payload = Encoding.UTF8.GetString(eventData.Body);
                var requestUri = "http://192.168.0.111:9999"; // or the URL you want to send data to

                var measurement = new Measurement
                {
                    DeviceId = deviceId,
                    Json = payload
                };

                try
                {
                    var message = await httpClient.PostAsJsonAsync (requestUri, measurement);

                    log.LogInformation (message.IsSuccessStatusCode ?
                        $"ForwardData: request sent successfully" :
                        $"ForwardData: request not sent successfully - {message.ReasonPhrase}");
                }
                catch (Exception ex)
                {
                    log.LogError (ex, $"Failed to send iot hub telemetry to {requestUri}");
                }
            }
        }

        public class Measurement
        {
            public string DeviceId { get; set; }
            public string Json { get; set; }
        }
    }
}
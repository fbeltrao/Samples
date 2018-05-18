using IoTHubTrigger = Microsoft.Azure.WebJobs.ServiceBus.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Fbeltrao.AzureFunctionExtensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;

namespace IoTDashboardWithSignalR.IoTHubPublisherFunction
{
    public static class IoTHub2SignalR
    {
        public class IoTHubPayload
        {
            public string deviceId { get; set; }
            public double temperature { get; set; }
            public double humidity { get; set; }
            public long time { get; set; }
            public string building { get; set; }
        }

        [FunctionName(nameof(Throubleshoot))]
        public static IActionResult Throubleshoot(
            [HttpTrigger] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info($"Http triggered");

            var res = new
            {
                signalr_accesskey = Environment.GetEnvironmentVariable("signalr_accesskey"),
                signalr_servicename = Environment.GetEnvironmentVariable("signalr_servicename"),
                iothub = Environment.GetEnvironmentVariable("iothub")
            };

            return new OkObjectResult(res);
        }


        [FunctionName("IoTHub2SignalR")]
        public static async System.Threading.Tasks.Task RunAsync(
                [IoTHubTrigger("messages/events", Connection = "iothub", ConsumerGroup = "$Default")] EventData[] messages,
                [SignalR(AccessKey = "%signalr_accesskey%", ServiceName = "%signalr_servicename%", Target = "measurement")]IAsyncCollector<SignalRMessage> signalRMessages,
                TraceWriter log)
        {

            var allMessages = new List<object>();
            var allBuildings = new Dictionary<string, List<object>>();


            log.Info($"Received {messages.Length} messages");

            foreach (var message in messages)
            {
                var jsonPayload = Encoding.UTF8.GetString(message.Body.Array);
                var payload = JsonConvert.DeserializeObject<IoTHubPayload>(jsonPayload);

                allMessages.Add(payload);
                if (!allBuildings.TryGetValue(payload.building, out var messagesForBuilding))
                {
                    messagesForBuilding = new List<object>();
                    allBuildings[payload.building] = messagesForBuilding;
                }
                messagesForBuilding.Add(payload);
            }
           
            foreach (var kv in allBuildings)
            {
                await signalRMessages.AddAsync(new SignalRMessage()
                {
                    Hub = "iot",
                    Groups = new string[] { kv.Key },
                    Arguments = new object[] { kv.Value }
                });
                log.Info($"Broadcasting {kv.Value.Count} messages to group '{kv.Key}'");
            }
        }
    }
}
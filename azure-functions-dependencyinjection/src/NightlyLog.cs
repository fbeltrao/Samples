using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionFunction
{
    /// <summary>
    /// Function that writes the incoming logs only in the night (21:00 - 05:00)
    /// </summary>
    public static class NightlyLog
    {
        [FunctionName(nameof(NightlyLog))]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] string logContent,
            [CosmosDB(databaseName: "functionTest",
            collectionName: "nightLogs",
            CreateIfNotExists = true,
            CollectionThroughput = 400,
            ConnectionStringSetting = "CosmosDBConnectionString")]
            IAsyncCollector<LogDocument> output,
            IDateTimeResolver dateTimeResolver,
            ILogger log)
        {
            var date = dateTimeResolver.Get();

            // only log what happens during the night
            if (date.Hour >= 21 || date.Hour < 5)
            {
                await output.AddAsync(new LogDocument { Log = logContent });
            }

            return new OkResult();
        }
    }
}


using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionFunction
{
    public static class PartsOfDay
    {
        /// <summary>
        /// Returns the part of the day based on http://learnersdictionary.com/qa/parts-of-the-day-early-morning-late-morning-etc
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <param name="dateTimeResolver"></param>
        /// <returns></returns>
        [FunctionName(nameof(PartsOfDay))]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, 
            ILogger log,
            IDateTimeResolver dateTimeResolver)
        {
            var date = dateTimeResolver.Get();

            if (date.Hour >= 5)
            {
                if (date.Hour < 12)
                    return "morning";

                if (date.Hour < 17)
                    return "afternoon";

                if (date.Hour < 21)
                    return "evening";
            }

            return "night";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreAITester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        static Random random = new Random();

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet]
        [Route("metric")]
        /// <summary>
        /// Creates custom event
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IActionResult Metric(string name = null, double? value = null)
        {
            name = name ?? "customMetric";
            if (!value.HasValue)
                value = random.NextDouble();
            
            var telemetry = new TelemetryClient();
            telemetry.TrackMetric(new MetricTelemetry(name, value.Value));

            return Content($"Metric {name} created with value {value}");
        }

        [HttpGet]
        [Route("event")]
        /// <summary>
        /// Creates custom event
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IActionResult Event(string name = null)
        {
            name = name ?? "customEvent";
            var telemetry = new TelemetryClient();
            telemetry.TrackEvent(name);

            return Content($"Event {name} created");
        }


        [HttpGet]
        [Route("error")]
        /// <summary>
        /// Throws an error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IActionResult Error(string message = null)
        {
            message = message ?? "Bug in the system";

            throw new Exception(message);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

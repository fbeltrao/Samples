using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICalculator calculator;
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ICalculator calculator, ILogger<ValuesController> logger)
        {
            this.calculator = calculator;
            this.logger = logger;
        }


        [HttpGet]
        [Route("sum/{v1}/{v2}")]
        public ActionResult<int> Sum(int v1, int v2)
        {
            try
            {
                return calculator.Sum(v1, v2);
            }
            catch (Exception ex)
            {
                // Logging as critical as we are not handling the exception                
                logger.LogCritical(ex, $"Error in {nameof(ValuesController)}.{nameof(Sum)}");
                throw;
            }
        }

        [HttpGet]
        [Route("div/{v1}/{v2}")]
        public ActionResult<int> Divide(int v1, int v2)
        {
            try
            {
                return calculator.Divide(v1, v2);
            }
            catch (DivideByZeroException)
            {
                // we know divided by zero can happen, therefore we handle it
                return BadRequest("Cannot divide by zero");
            }
            catch (Exception ex)
            {
                // Logging as critical as we are not handling the exception
                logger.LogCritical(ex, $"Error in {nameof(ValuesController)}.{nameof(Divide)}");
                throw;
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                logger.LogInformation($"{nameof(ValuesController)}.{nameof(Get)} called");
                                
                return new string[] { "value1", "value2" };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(Get)}");
                return new string[0];
            }
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

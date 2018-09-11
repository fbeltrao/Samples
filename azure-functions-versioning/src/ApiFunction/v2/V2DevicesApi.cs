using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ApiFunction.Data;
using System.Linq;
using ApiFunction.v2.Models;
using System.Collections.Generic;

namespace ApiFunction.v2
{
    public static class V2DevicesApi
    {
        [FunctionName(nameof(V2List))]
        public static IActionResult V2List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v2/devices")]
            HttpRequest req, 
            ILogger log)
        {
            var allDevices = DeviceRepository
                .Get()
                .GetAll()
                .Select(x => new DeviceModel
                {
                    DeviceId = x.DeviceId,
                    CreationDate = x.CreationDate,
                    Location = x.Location,
                    Department = x.Department,
                });

            return new JsonResult(allDevices);
        }

        [FunctionName(nameof(V2Get))]
        public static IActionResult V2Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v2/devices/{deviceId}")]
            HttpRequest req,
            ILogger log,
            string deviceId)
        {
            var device = DeviceRepository
                .Get()
                .Get(deviceId);

            if (device == null)
                return new NotFoundResult();

            var deviceModel = new DeviceModel
                {
                    DeviceId = device.DeviceId,
                    CreationDate = device.CreationDate,
                    Location = device.Location,
                    Department = device.Department,
                };

            return new OkObjectResult(deviceModel);
        }

        [FunctionName(nameof(V2Post))]
        public static IActionResult V2Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v2/devices")]
            DeviceModel input,
            ILogger log)
        {
            var device = new Device
            {
                DeviceId = input.DeviceId,
                CreationDate = input.CreationDate,
                Location = input.Location,
                Department = input.Department,
            };

            try
            {
                DeviceRepository
                    .Get()
                    .Create(device);

                return new AcceptedResult();
            }
            catch (DuplicateDeviceException)
            {
                return new BadRequestResult();
            }
        }

        [FunctionName(nameof(V2Put))]
        public static IActionResult V2Put(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v2/devices/{deviceId}")]
            DeviceModel input,
            ILogger log,
            string deviceId)
        {
            var device = new Device
            {
                DeviceId = deviceId,
                CreationDate = input.CreationDate,
                Location = input.Location,
                Department = input.Department,
            };

            try
            {
                DeviceRepository
                    .Get()
                    .Update(deviceId, device);


                return new AcceptedResult();
            }
            catch (DeviceNotFound)
            {
                return new NotFoundResult();
            }
        }
    }
}

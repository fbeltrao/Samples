using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ApiFunction.Data;
using System.Linq;
using ApiFunction.v1.Models;
using System.Collections.Generic;

namespace ApiFunction.v1
{
    public static class V1DevicesApi
    {
        [FunctionName(nameof(V1List))]
        public static IEnumerable<DeviceModel> V1List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/devices")]
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
                });

            return allDevices;
        }

        [FunctionName(nameof(V1Get))]
        public static IActionResult V1Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/devices/{deviceId}")]
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
                };

            return new OkObjectResult(deviceModel);
        }

        [FunctionName(nameof(V1Post))]
        public static IActionResult V1Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/devices")]
            DeviceModel input,
            ILogger log)
        {
            var device = new Device
            {
                DeviceId = input.DeviceId,
                CreationDate = input.CreationDate,
                Location = input.Location
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

        [FunctionName(nameof(V1Put))]
        public static IActionResult V1Put(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/devices/{deviceId}")]
            DeviceModel input,
            ILogger log,
            string deviceId)
        {
            var device = new Device
            {
                DeviceId = deviceId,
                CreationDate = input.CreationDate,
                Location = input.Location
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

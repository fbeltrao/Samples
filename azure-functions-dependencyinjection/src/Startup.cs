using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(DependencyInjectionFunction.Startup), "DependencyInjectionFunctionStartup")]

namespace DependencyInjectionFunction
{
    /// <summary>
    /// Startup called by WebJob host
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        void ConfigureServices(IServiceCollection services)
        {
            // Setup the Dependency Injection Handler
            services.AddSingleton<IBindingProvider, DependencyInjectionBindingProvider>();

            // Setup custom application DI
            services.AddSingleton<IDateTimeResolver, DateTimeResolver>();                        
        }
    }
}

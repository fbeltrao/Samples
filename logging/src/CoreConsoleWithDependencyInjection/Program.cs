#define USE_SERILOG
using Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace CoreConsoleWithDependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {            
            var services = new ServiceCollection();
            ConfigureServices(services);
            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.GetService<Application>().Run();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            services.AddSingleton(loggerFactory);
            ApplicationLogging.LoggerFactory = loggerFactory;

            var configuration = GetConfiguration();
            services.AddSingleton<IConfigurationRoot>(configuration);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            });


#if USE_SERILOG
            var logger = new LoggerConfiguration()                
                .ReadFrom.Configuration(configuration)                
                .CreateLogger();
            loggerFactory.AddSerilog(logger);
#endif

            
            services.AddTransient<Application>();
            services.AddTransient<Calculator>();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
#if USE_SERILOG
                .AddJsonFile("appsettings.serilog.json", optional: true)
#else
#endif
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
    }
}

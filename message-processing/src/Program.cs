using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessing
{   
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var hostBuilder = new HostBuilder()
                    .ConfigureLogging((hostContext, configLogging) =>
                    {
                        configLogging.AddConsole();                    
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        if (hostContext.HostingEnvironment.IsDevelopment())
                        {
                            // Development service configuration
                        }
                        else
                        {
                            // Non-development service configuration
                        }

                        //services.AddSingleton<ILoraAntenna, UdpLoraAntenna>();

                        // Using fake lora antenna that sends messages every x seconds
                        services.AddSingleton<ILoraAntenna, FakeLoraAntenna>();
                        services.AddSingleton<IMediator, Mediator>();
                        services.AddSingleton<ILoraMessageFactory, LoraMessageFactory>();
                        services.AddScoped<MessageHandler<LoraJoinMessage>, LoraJoinMessageHandler>();
                        services.AddScoped<MessageHandler<LoraTelemetryMessage>, LoraTelemetryMessageHandler>();
                        services.AddHostedService<LoraNetworkService>();
                    });

                await hostBuilder.RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

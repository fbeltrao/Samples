using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionFunction
{
    /// <summary>
    /// Dependency Injection binding provider
    /// Minimal implementation only covering interfaces without any attribute
    /// </summary>
    public class DependencyInjectionBindingProvider : IBindingProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public DependencyInjectionBindingProvider(IServiceProvider serviceProvider, ILogger<DependencyInjectionBindingProvider> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var parameter = context.Parameter;            
            if (parameter.ParameterType.IsInterface)
            {
                // Ignore parameters with attributes
                if (parameter.GetCustomAttributes(false).Length == 0)
                {
                    var registeredType = serviceProvider.GetService(parameter.ParameterType);
                    if (registeredType != null)
                    {
                        logger.LogInformation("Implementation for {type} found: {concreteType}", parameter.ParameterType.Name, registeredType.GetType().Name);

                        return Task.FromResult<IBinding>(new DependencyInjectionBinding(parameter.Name, parameter.ParameterType, registeredType));
                    }
                    else
                    {
                        logger.LogInformation("Implementation for {type} not found. Did you registers in the IServiceCollection", parameter.ParameterType.Name);
                    }
                }           
            }

            return Task.FromResult<IBinding>(null);
        }
    }
}

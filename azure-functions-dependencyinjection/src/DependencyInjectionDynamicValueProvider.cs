using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace DependencyInjectionFunction
{
    /// <summary>
    /// Returns a dynamic value, getting from <see cref="IServiceProvider"/>
    /// This we respect scoped objects
    /// </summary>
    internal class DependencyInjectionDynamicValueProvider : IValueProvider
    {
        private readonly Type type;
        private readonly IServiceProvider serviceProvider;
        private readonly string invokeString;

        internal DependencyInjectionDynamicValueProvider(Type type, IServiceProvider serviceProvider, string invokeString)
        {
            this.type = type;
            this.serviceProvider = serviceProvider;
            this.invokeString = invokeString;
        }

        public Type Type => this.type;

        public Task<object> GetValueAsync() => Task.FromResult(this.serviceProvider.GetService(type));        

        public string ToInvokeString() => this.invokeString;
    }
}
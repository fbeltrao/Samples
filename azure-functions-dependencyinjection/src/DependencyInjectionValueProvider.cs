using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace DependencyInjectionFunction
{
    internal class DependencyInjectionValueProvider : IValueProvider
    {
        private readonly Type type;
        private readonly object value;
        private readonly string invokeString;

        internal DependencyInjectionValueProvider(Type type, object value, string invokeString)
        {
            this.type = type;
            this.value = value;
            this.invokeString = invokeString;
        }

        public Type Type => this.type;

        public Task<object> GetValueAsync() => Task.FromResult(this.value);        

        public string ToInvokeString() => this.invokeString;
    }
}
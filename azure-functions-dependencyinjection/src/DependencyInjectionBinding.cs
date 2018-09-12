using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace DependencyInjectionFunction
{
    internal class DependencyInjectionBinding : IBinding
    {
        private readonly string name;
        private readonly Type type;
        private readonly object value;

        /// <summary>
        /// Creates a new instance of <see cref="DependencyInjectionBinding"/>
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="type">The property type</param>
        /// <param name="value">The resolved property value</param>
        internal DependencyInjectionBinding(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }

        public bool FromAttribute => false;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor { Name = this.name };
        
        public Task<IValueProvider> BindAsync(BindingContext context) => BindAsync(this.value, context.ValueContext);

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.type.IsAssignableFrom(value.GetType()))
            {
                return Task.FromResult<IValueProvider>(new DependencyInjectionValueProvider(this.type, this.value, this.name));
            }

            return null;
        }
    }
}

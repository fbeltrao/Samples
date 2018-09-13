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
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Creates a new instance of <see cref="DependencyInjectionBinding"/>
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="type">The property type</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
        internal DependencyInjectionBinding(string name, Type type, IServiceProvider serviceProvider)
        {
            this.name = name;
            this.type = type;
            this.serviceProvider = serviceProvider;
        }

        public bool FromAttribute => false;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor { Name = this.name };
        
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult<IValueProvider>(new DependencyInjectionDynamicValueProvider(this.type, this.serviceProvider, this.name));            
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.type.IsAssignableFrom(value.GetType()))
            {
                return Task.FromResult<IValueProvider>(new DependencyInjectionConstantValueProvider(this.type, value, this.name));
            }

            return null;
        }
    }
}

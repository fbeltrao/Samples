using DependencyInjectionFunction;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionFunctionTest
{
    public class FixDateTimeResolver : IDateTimeResolver
    {
        private readonly DateTime dateTime;

        public FixDateTimeResolver(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public DateTime Get() => this.dateTime;
    }
}

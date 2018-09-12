using System;

namespace DependencyInjectionFunction
{
    public class DateTimeResolver : IDateTimeResolver
    {
        public DateTime Get() => DateTime.UtcNow;
    }
}
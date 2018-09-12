using Microsoft.Azure.WebJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyInjectionFunctionTest
{
    public class TestAsyncCollector<T> : IAsyncCollector<T>, IEnumerable<T>
    {
        private readonly List<T> innerList;

        public TestAsyncCollector()
        {
            this.innerList = new List<T>();
        }

        public Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.innerList.Add(item);
            return Task.FromResult(0);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public IEnumerator<T> GetEnumerator() => this.innerList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerList.GetEnumerator();
    }
}

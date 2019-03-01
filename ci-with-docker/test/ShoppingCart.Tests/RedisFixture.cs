using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Tests
{
    public class RedisFixture : IDisposable
    {
        private ConnectionMultiplexer redis;
        public IDatabase Database { get; }

        public RedisFixture()
        {
            this.redis = ConnectionMultiplexer.Connect("localhost:6379");
            this.Database = redis.GetDatabase();
        }

        public void Dispose()
        {
            this.redis?.Dispose();
            this.redis = null;
        }
    }
}

using System;
using Xunit;

namespace ShoppingCart.Tests
{
    public class ShoppingBasketTests : IClassFixture<RedisFixture>
    {
        private readonly RedisFixture redisFixture;

        public ShoppingBasketTests(RedisFixture redisFixture)
        {
            this.redisFixture = redisFixture;
        }

        [Fact]
        public void Add_Should_Have_One_Item()
        {
            var target = new Basket(Guid.NewGuid().ToString(), this.redisFixture.Database);
            Assert.Equal(0, target.Count());

            target.AddItem("shoes");

            Assert.Equal(1, target.Count());
        }


        [Fact]
        public void Clear_Should_Have_Remove_All_Items()
        {
            var target = new Basket(Guid.NewGuid().ToString(), this.redisFixture.Database);
            target.AddItem("shoes");
            target.AddItem("shoes2");
            target.AddItem("shoes3");

            target.Clear();
            Assert.Equal(0, target.Count());
        }
    }
}

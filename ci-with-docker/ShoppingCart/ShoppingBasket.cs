using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart
{
    public class Basket
    {
        private readonly string id;
        private readonly IDatabase db;

        public Basket(string id, IDatabase db)
        {
            this.id = id;
            this.db = db;

        }

        string CacheKey() => $"basket:{id}";

        public void AddItem(string item)
        {
            db.ListRightPush(CacheKey(), item);
        }

        public long Count()
        {
            return db.ListLength(CacheKey());
        }


        public void Clear()
        {
            db.KeyDelete(CacheKey());
        }
    }
}

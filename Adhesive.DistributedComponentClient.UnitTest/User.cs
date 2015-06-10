using System;
using System.Collections.Generic;

namespace Adhesive.DistributedComponentClient.UnitTest
{
    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public List<Order> Orders { get; set; }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user == null) return false;
            return user.Age == this.Age && user.Name == this.Name
                && user.Orders.Count == this.Orders.Count;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public User()
        {
        }

        public User(int count)
        {
            Name = "你妈妈" + count;
            Age = count;
            Orders = new List<Order>();
            for (int i = 0; i < 10; i++)
            {
                Orders.Add(new Order
                {
                    Name = "商品名" + i,
                    Price = 100.23 * i,
                    SubmitTime = DateTime.Now.AddMinutes(i),
                });
            }
        }
    }

    public class Order
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public DateTime SubmitTime { get; set; }
    }
}

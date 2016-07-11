using System;
using System.Collections.Generic;

namespace Samples.Complete.Models
{
    class Order
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }

        public string Id { get; set; }

        public DateTime OrderDate { get; set; }

        public Address Address { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public decimal AmountPaidByCard { get; set; }

        public string TransactionCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Complete.Models
{
    class OrderItem
    {
        public string ProductCode { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Delivery { get; set; }

        public PaymentType PaymentType { get; set; }
    }
}

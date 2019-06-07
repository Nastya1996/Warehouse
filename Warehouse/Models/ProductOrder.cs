using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductOrder
    {

        public string Id { get; set;}
        public uint Count { get; set; }
        public decimal Price { get; set; }
        public decimal Sale { get; set; }
        public decimal FinallyPrice { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string ProductManagerId { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }
    }
}

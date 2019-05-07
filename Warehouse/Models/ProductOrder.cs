using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductOrder
    {
        public string Id { get; set;}
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string ProductId { get; set; }
    }
}

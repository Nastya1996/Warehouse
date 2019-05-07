using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public uint Count { get; set; }
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
    }
}

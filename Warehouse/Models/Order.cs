using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Order
    {
        public string Id { get; set; }
        public uint Count { get; set; }
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public Product Product { get; set; }
        public Customer Customer { get; set; }
        public IList<ProductOrder> ProductOrders { get; set; }
        
    }
}

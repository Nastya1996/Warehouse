using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Order
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public decimal Sale { get; set; }
        public decimal FinallPrice { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public IList<ProductOrder> ProductOrders { get; set; }
        public OrderType OrderType { get; set; }
        public string WareHouseId { get; set; }
        //public bool IsSelled { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        //productOrderId and sale
        public Dictionary<string, decimal> ProductOrderAndSale { get; set; }
    }
}

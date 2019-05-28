using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Basket
    {
        public string Id { set; get; }
        public uint Quantity { get; set; }
        public string ProductManagerId { set; get; }
        public string UserId { set; get; }
        public ProductManager ProductManager { set; get; }
    }
}

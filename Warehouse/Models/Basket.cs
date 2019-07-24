using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Basket
    {
        public string Id { get; set; }
        public uint Count { get; set; }
        public DateTime AddDate { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { set; get; }
        public AppUser User { get; set; }
        public string WarehouseId { get; set; }
        public WareHouse WareHouse { get; set; }
 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class WriteOut
    {
        public string Id { get; set; }
        public uint Count { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string WarehouseId { get; set; }
        public WareHouse Warehouse { get; set; }
    }
}

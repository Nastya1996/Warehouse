using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductManager
    {
        public string Id { get; set; }
        
        public uint Count { get; set; }
        public uint CurrentCount { get; set; }
        public decimal ReceiptPrice { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime AddedDate { get; } = DateTime.Now;
        public string UserId { get; set; }

        public string WareHouseId { get; set; }
        public WareHouse WareHouse { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}

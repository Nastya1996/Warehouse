using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductManager
    {
        public string ProductId { get; set; }
        public uint Count { get; set; }
        public uint CurrentCount { get; set; }
        public decimal ReceiptPrice { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime AddedDate { get; set; }
        public string UserId { get; set; }
        public string WareHouseId { get; set; }

    }
}

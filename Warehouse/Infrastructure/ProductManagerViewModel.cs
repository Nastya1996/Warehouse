using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Infrastructure
{
    public class ProductManagerViewModel
    {
        public string TypeId { get; set; }
        public string ProductId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string WarehouseID { get; set; }
    }
}

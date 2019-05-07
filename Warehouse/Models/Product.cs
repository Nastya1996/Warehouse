using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { set; get; }
        public int Barcode { set; get; }
        public string TypeId { set; get; }
        public string UnitId { set; get; }
    }
}

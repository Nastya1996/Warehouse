using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductMove
    {
        public string Id { get; set; }

        public string BeforeId { get; set; }
        [ForeignKey("BeforeId")]
        public WareHouse Before { get; set; }


        public string AfterId { get; set; }
        [ForeignKey("AfterId")]
        public WareHouse After { get; set; }


        public string UserId { get; set; }
        public AppUser User { get; set;}


        public DateTime Date { get; set; }
        public uint Count { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string TypeId { get; set; }
        public ProductType Type { get; set; }
    }
}

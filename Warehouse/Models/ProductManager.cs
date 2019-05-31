using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductManager
    {
        [Required]
        public string Id { get; set; }
        [RegularExpression(@"^[1-9]{1}\d*$", ErrorMessage = "*Unacceptable symbols")]
        [Range(0, int.MaxValue, ErrorMessage = "*The amount goes beyond what is permitted")]
        public uint Count { get; set; }
        public uint CurrentCount { get; set; }
        [RegularExpression(@"^[1-9]{1}\d*$", ErrorMessage = "*Unacceptable symbols")]
        [Range(0, double.MaxValue, ErrorMessage = "*Price goes beyond what is permitted")]
        public decimal ReceiptPrice { get; set; }
        [RegularExpression(@"^[1-9]{1}\d*$", ErrorMessage = "*Unacceptable symbols")]
        [Range(0, double.MaxValue, ErrorMessage = "*Price goes beyond what is permitted")]
        public decimal SalePrice { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime AddDate { get; set; }


        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string WareHouseId { get; set; }
        public WareHouse WareHouse { get; set; }
        [Required]
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}

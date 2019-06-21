using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class ProductManager
    {
        
        public string Id { get; set; }
        [Required(ErrorMessage = "*The field is not filled")]
        [RegularExpression("([1-9][0-9]{0,15})", ErrorMessage = "*Invalid value")]
        public uint Count { get; set; }
        public uint CurrentCount { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [RegularExpression("([1-9][0-9]{0,15})", ErrorMessage = "*Invalid value")]
        public decimal ReceiptPrice { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [RegularExpression("([1-9][0-9]{0,15})", ErrorMessage = "*Invalid value")]
        public decimal SalePrice { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string WareHouseId { get; set; }
        public WareHouse WareHouse { get; set; }
        [Required]
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public bool IsWriteOut { get; set; } = false;
    }
}

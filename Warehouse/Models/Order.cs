using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Order
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [Range(0, double.MaxValue, ErrorMessage = "*Price goes beyond what is permitted")]
        [RegularExpression(@"^[1-9]{1}\d*$", ErrorMessage = "Unacceptable symbols")]
        public decimal Sum { get; set; }

        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public IList<ProductOrder> ProductOrders { get; set; }
    }
}

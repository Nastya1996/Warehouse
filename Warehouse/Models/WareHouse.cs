using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class WareHouse
    {
        public string Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        [RegularExpression("[a-zA-Z]{3,}", ErrorMessage = "Only symbols")]
        public string Address { get; set; }

    }
}

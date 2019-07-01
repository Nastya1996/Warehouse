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

        [StringLength(20, MinimumLength = 3, ErrorMessage = "*The number of warehouse must be between 3 and 20 characters")]
        [Required(ErrorMessage = "*The field is not filled")]
        public string Number { get; set; }
        
        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20, MinimumLength =3, ErrorMessage = "*The adress must be between 3 and 20 characters")]
        public string Address { get; set; }

        public bool IsActive { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Unit
    {
        public string Id { set; get; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(10,ErrorMessage = "*The unit name must be up to 10 characters long")]
        public string Name { set; get; }
    }
}

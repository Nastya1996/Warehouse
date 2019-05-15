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
        [Required(ErrorMessage = "Не указано имя")]
        [RegularExpression("[a-zA-Z]{1,}")]
        public string Name { set; get; }
    }
}

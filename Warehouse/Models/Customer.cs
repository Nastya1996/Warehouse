using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Customer
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20,MinimumLength =3,ErrorMessage = "*The name must be between 3 and 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "*The name must be between 3 and 20 characters")]
        public string Surname { get; set; }

        [StringLength(20, MinimumLength =9,ErrorMessage = "*Wrong format")]
        [Required(ErrorMessage = "*The field is not filled")]
        [RegularExpression(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$",ErrorMessage = "*Wrong format")]
        public string Phone { get; set; }

    }
}

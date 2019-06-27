using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace Warehouse.Models
{
    public class ProductType
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20, MinimumLength =3, ErrorMessage = "*The type name must be between 3 and 20 characters")]
        public string Name { get; set; }
        public bool IsActive { set; get; }
        public override string ToString() => Name;
    }
}

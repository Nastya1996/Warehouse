using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Infrastructure;
namespace Warehouse.Models
{
    public class ProductType
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20, MinimumLength =3, ErrorMessage = "*The type name must be between 3 and 20 characters")]
        [Remote("TypeAvailability", "ProductType")]
        [TypeAvailability]
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}

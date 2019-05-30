using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Infrastructure;

namespace Warehouse.Models
{
    public class Product
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(20,MinimumLength =3,ErrorMessage = "*The product name must be between 3 and 20 characters")]
        [Remote("ProductAvailability","Product")]
        [ProductAvailability]
        public string Name { set; get; }

        [Required(ErrorMessage = "*The field is not filled")]
        [StringLength(13,MinimumLength =13,ErrorMessage = "*The barcode length should be 13 characters")]
        public string Barcode { get; set; }

        public string ProductTypeId { set; get; }
        public string UnitId { set; get; }
        public ProductType ProductType { set; get; }
        public Unit Unit { set; get; }

        public bool IsActive { set; get; }
    }
}

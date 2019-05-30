using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Data;

namespace Warehouse.Infrastructure
{
    public class ProductAvailability: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string name = value?.ToString() ?? null;
            if (name != null)
            {
                ApplicationDbContext _context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
                name = name.Trim();
                var productName = _context.Products.FirstOrDefault(p => p.Name == name);
                if (productName != null)
                    return new ValidationResult("*The name of product is available in the database");
            }
            return ValidationResult.Success;
        }
    }
}

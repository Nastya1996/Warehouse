using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Data;

namespace Warehouse.Infrastructure
{
    public class TypeAvailability:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string name = value?.ToString() ?? null;
            if (name != null)
            {
                ApplicationDbContext _context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
                var typeName = _context.Types.FirstOrDefault(t => t.Name == name);
                if (typeName != null)
                    return new ValidationResult("*This type of product is available in the database");
            }
            return ValidationResult.Success;
            
        }
    }
}

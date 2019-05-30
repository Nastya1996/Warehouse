using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Basket
    {
        public string Id { set; get; }
        public string UserId { set; get; }
        public AppUser User { get; set; }
        public IList<ProductBasket> ProductBaskets { get; set; } = new List<ProductBasket>();
    }
}

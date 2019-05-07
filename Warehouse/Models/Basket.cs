using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class Basket
    {
        public string Id { set; get; }
        public string ProductManagerId { set; get; }
        public string UserId { set; get; }
    }
}

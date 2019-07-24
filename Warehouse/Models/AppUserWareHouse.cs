using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class AppUserWareHouse
    {
        public string Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string WareHouseId { get; set; }
        public WareHouse WareHouse { get; set; }
    }
}

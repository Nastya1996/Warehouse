using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Models;
namespace Warehouse.Infrastructure
{
    public class UsersViewModel
    {
        public string Name { get; set; } = "";
        public string RoleId { get; set; }
        [NotMapped]
        public List<string> WareHouses { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

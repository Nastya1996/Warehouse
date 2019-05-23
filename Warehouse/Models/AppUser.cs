using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Warehouse.Models
{
	public class AppUser : IdentityUser
	{
		public string Name { get; set; }
		public string SurName { get; set; }
		[DataType(DataType.Date)]
		public DateTime BirthDate { get; set; }

		public string WarehouseId { get; set; }
		public WareHouse Warehouse { get; set; }
	}
}

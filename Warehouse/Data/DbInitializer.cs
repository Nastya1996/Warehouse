using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Data
{
	public static class DbInitializer
	{
		public static void Initialize(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager )	{
			string[] roles = { "Admin", "Worker", "Storekeeper", "Report" };
			string pass = "_Aa123456";
			foreach (string role in roles)
			{
				if (!roleManager.RoleExistsAsync(role).Result)
					roleManager.CreateAsync(new IdentityRole { Name = role }).Wait();
			}
			if (userManager.FindByEmailAsync("warehouseadmin@gmail.com").Result == null)
			{
				AppUser user = new AppUser
				{
					Name = "Program",
					SurName = "Administrator",
					Email = "warehouseadmin@gmail.com",
					UserName = "warehouseadmin@gmail.com",
					LockoutEnabled = false
				};
				IdentityResult res =  userManager.CreateAsync(user, pass).Result;
				if (res.Succeeded)
				{
					userManager.AddToRoleAsync(user, "Admin").Wait();
				}
			}
			

			
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Warehouse.Models;
using Warehouse.Data;

namespace Warehouse
{
    public class Program
    {
        public static void Main(string[] args)
        {
			IWebHost host = CreateWebHostBuilder(args).Build();
			using (IServiceScope scope = host.Services.CreateScope())
			{
				try
				{
					var userScope = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
					var roleScope = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					DbInitializer.Initialize(roleScope, userScope);
				}
				catch(Exception e)
				{
					throw e;
				}
			}

			host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

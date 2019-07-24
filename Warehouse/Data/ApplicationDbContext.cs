using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Warehouse.Models;
namespace Warehouse.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<ProductType> Types { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<ProductManager> ProductManagers {get;set;}
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductCustomer> ProductCustomers { get; set; }
        public DbSet<WareHouse> Warehouses { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { set; get; }
        public DbSet<Unit> Units { set; get; }
        public DbSet<Basket> Baskets { set; get; }
        public DbSet<FileModelImg> Files { get; set; }
        public DbSet<WriteOut> WriteOuts { get; set; }
        public DbSet<AppUserWareHouse> AppUserWareHouses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductOrders)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}

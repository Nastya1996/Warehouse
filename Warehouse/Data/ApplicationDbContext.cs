﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Warehouse.Models;

namespace Warehouse.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ProductCustomer> ProductCustomers { get; set; }
        public DbSet<WareHouse> Warehouses { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { set; get; }
        public DbSet<Unit> Units { set; get; }
        public DbSet<Basket> Baskets { set; get; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}

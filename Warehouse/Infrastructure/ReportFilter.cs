using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Models;

namespace Warehouse.Infrastructure
{
    public class ReportViewModel
    {
        public string ProductId { get; set; }
        public string TypeId { get; set; }
        public string UserId { get; set; }
        public string Deal { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; } = DateTime.Now.Date;
        public DateTime DateFrom { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public IQueryable<ProductManager> ProductManagers { get; set; }
        public IQueryable<ProductOrder> ProductOrders { get; set; }
    }
}

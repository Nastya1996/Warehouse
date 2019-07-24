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
        public ReportViewModel(IDictionary<string, string> data)
        {
            var keys = data.Keys;
            if (keys.Contains("Deal")) Deal = data["Deal"];
            if (keys.Contains("DateTo")) DateTo =Convert.ToDateTime(data["DateTo"]);
            if (keys.Contains("DateFrom")) DateFrom = Convert.ToDateTime(data["DateFrom"]);
            if (keys.Contains("page")) page = Convert.ToInt32(data["page"]);
            if (keys.Contains("pageSize")) pageSize = Convert.ToInt32(data["pageSize"]);
            if (keys.Contains("TypeId")) TypeId = data["TypeId"];
            if (keys.Contains("UserId")) UserId = data["UserId"];
            if (keys.Contains("ProductId")) ProductId = data["ProductId"];
            if (keys.Any(k=>k.Contains("Warehouse")))
            {
                WarehouseId = data.Where(d => d.Key.Contains("Warehouse")).Select(d=>d.Value).ToList();
            }
        }
        public string ProductId { get; set; }
        public string TypeId { get; set; }
        public string UserId { get; set; }
        public IList<string> WarehouseId { get; set; }
        public string Deal { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; } = DateTime.Now.Date;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}

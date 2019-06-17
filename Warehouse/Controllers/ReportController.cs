using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;

namespace Warehouse.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(DateTime dateFrom, DateTime dateTo, string pType, string productName, string worker,  string gorcarq)
        {
            //&& p.Product.ProductTypeId == pType && p.ProductId == productName && p.UserId == worker
            if (dateTo == DateTime.MinValue)
                dateTo = DateTime.Now;
            if (worker == null)
                worker = "";
            if (pType == null)
                pType = "";
            if (productName == null)
                productName = "";

            var pm = _context.ProductManagers.Where(p=>p.ReceiptDate >= dateFrom && p.AddDate <= dateTo && p.UserId.Contains(worker, StringComparison.InvariantCultureIgnoreCase) && p.Product.ProductTypeId.Contains(pType, StringComparison.InvariantCultureIgnoreCase) && p.ProductId.Contains(productName, StringComparison.InvariantCultureIgnoreCase))
                .Include(prodManager => prodManager.User).Include(pM => pM.Product.ProductType).Include(w => w.WareHouse).ToList();
            var types = _context.Types.ToList();
            ViewBag.prodType = new SelectList(types, "Id", "Name");
            var products = _context.Products.ToList();
            ViewBag.prodName = new SelectList(products, "Id", "Name");
            var users = _context.AppUsers.ToList();
            ViewBag.workers = new SelectList(users, "Id", "Name");
            return View(pm);
        }
    }
}
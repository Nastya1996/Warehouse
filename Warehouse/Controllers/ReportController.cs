using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PagedList.Core;
using PagedList.Core.Mvc;
using Warehouse.Data;
using Warehouse.Infrastructure;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(ReportViewModel reportFilter)
        {
            reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? _context.ProductManagers.Min(d => d.Date)
                                                : reportFilter.DateFrom;
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var wareHouseId = user.WarehouseId;
            ViewBag.Names = new SelectList(_context.Products, "Id", "Name");
            ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users.Where(u => u.WarehouseId == wareHouseId),"Id","UserName");
            if (reportFilter.Deal == null)
            {
                return View(reportFilter);
            }
            if (reportFilter.Deal.Equals("Import"))
            {
                var queryImport = _context.ProductManagers.Where(pm => pm.WareHouseId == wareHouseId)
                                                .Include(pm => pm.Product.ProductType)
                                                .Include(pm => pm.User).AsQueryable();
                if (reportFilter.ProductId!=null)
                    queryImport = queryImport.Where(x => x.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryImport = queryImport.Where(x => x.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryImport = queryImport.Where(x => x.UserId == reportFilter.UserId);
                queryImport = queryImport.Where(pm => pm.Date >= reportFilter.DateFrom && pm.Date <= reportFilter.DateTo);
                reportFilter.ProductManagers = queryImport;
                ViewBag.paged = new PagedList<ProductManager>(queryImport, reportFilter.Page, reportFilter.PageSize);
            }
            if (reportFilter.Deal.Equals("Saled"))
            {
                var querySaled = _context.ProductOrders.Include(po => po.Product.ProductType)
                                                       .Include(po => po.Order).AsQueryable();
                if (reportFilter.ProductId != null)
                    querySaled = querySaled.Where(po => po.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    querySaled = querySaled.Where(po => po.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    querySaled = querySaled.Where(po => po.Order.UserId == reportFilter.UserId);
                querySaled = querySaled.Where(po => po.Order.Date >= reportFilter.DateFrom && po.Order.Date <= reportFilter.DateTo);
                reportFilter.ProductOrders = querySaled;
                ViewBag.paged = new PagedList<ProductOrder>(querySaled, reportFilter.Page, reportFilter.PageSize);
            }
            return View(reportFilter);
        }
        public IActionResult ExcelExport(ReportViewModel reportFilter)
        {
            
            reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? _context.ProductManagers.Min(d => d.Date)
                                                : reportFilter.DateFrom;
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var wareHouseId = user.WarehouseId;
            ViewBag.Names = new SelectList(_context.Products, "Id", "Name");
            ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users.Where(u => u.WarehouseId == wareHouseId), "Id", "UserName");
            if (reportFilter.Deal == null)
            {
                return RedirectToAction("Index", new Dictionary<string, ReportViewModel> { { "reportFilter", reportFilter } });
            }
            if (reportFilter.Deal.Equals("Import"))
            {
                var queryImport = _context.ProductManagers.Where(pm => pm.WareHouseId == wareHouseId)
                                                .Include(pm => pm.Product.ProductType)
                                                .Include(pm => pm.User).AsQueryable();
                if (reportFilter.ProductId != null)
                    queryImport = queryImport.Where(x => x.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryImport = queryImport.Where(x => x.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryImport = queryImport.Where(x => x.UserId == reportFilter.UserId);
                queryImport = queryImport.Where(pm => pm.Date >= reportFilter.DateFrom && pm.Date <= reportFilter.DateTo);
                reportFilter.ProductManagers = queryImport;
                return ExportProductManager(queryImport.ToList());
            }
            if (reportFilter.Deal.Equals("Saled"))
            {
                var querySaled = _context.ProductOrders.Include(po => po.Product.ProductType)
                                                       .Include(po => po.Order.User).AsQueryable();
                if (reportFilter.ProductId != null)
                    querySaled = querySaled.Where(po => po.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    querySaled = querySaled.Where(po => po.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    querySaled = querySaled.Where(po => po.Order.UserId == reportFilter.UserId);
                querySaled = querySaled.Where(po => po.Order.Date >= reportFilter.DateFrom && po.Order.Date <= reportFilter.DateTo);
                reportFilter.ProductOrders = querySaled;
                return ExportProductOrder(querySaled.ToList());
            }
            return RedirectToAction("Index", new Dictionary<string, ReportViewModel> { { "reportFilter", reportFilter } });

        }
        public FileStreamResult ExportProductManager(IList<ProductManager> list)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loading");
                sheet.Cells.LoadFromCollection(list.Select(pm => new {
                    ProductName = pm.Product.Name,
                    ProductType = pm.Product.ProductType.Name,
                    pm.Count,
                    pm.ReceiptPrice,
                    ReciveDate=pm.Date.ToString("d"),
                    UserName = pm.User.Name
                }), true);
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"InputReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public FileStreamResult ExportProductOrder(IList<ProductOrder> list)
        {
            var stream = new MemoryStream();
            using(var package=new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loading");
                sheet.Cells.LoadFromCollection(list.Select(po => new {
                    ProductName = po.Product.Name,
                    ProductType = po.Product.ProductType.Name,
                    po.Count,
                    po.FinallyPrice,
                    SaledDate=po.Order.Date.ToString("d"),
                    UserName = po.Order.User.Name
                }), true);
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"SaledReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        
    }
}
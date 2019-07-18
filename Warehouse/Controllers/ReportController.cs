using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Report")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportFilter"></param>
        /// <returns></returns>
        public IActionResult Index(ReportViewModel reportFilter)
        {
            if (!FilterValid()) return BadRequest();
            reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? _context.ProductManagers.Min(d => d.Date)
                                                : reportFilter.DateFrom;
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var wareHouseId = user.WarehouseId;
            ViewBag.Names = new SelectList(_context.Products, "Id", "Name");
            ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users.Where(u=>u.Id!=user.Id),"Id","UserName");
            if (reportFilter.Deal == null)
            {
                return View(reportFilter);
            }
            if (reportFilter.Deal.Equals("0"))
                Import(reportFilter);
            else if (reportFilter.Deal.Equals("1"))
                Saled(reportFilter);
            else if (reportFilter.Deal.Equals("2"))
                Export(reportFilter);
            else return BadRequest();
            return View(reportFilter);
        }


        [NonAction]
        void Import(ReportViewModel reportFilter)
        {
            var wareHouseId = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value).WarehouseId;
            var queryImport = _context.ProductManagers.Where(pm => pm.WareHouseId == wareHouseId)
                                               .Include(pm => pm.Product.ProductType)
                                               .Include(pm => pm.User).AsQueryable();
            if (reportFilter.ProductId != null)
                if (_context.Products.Find(reportFilter.ProductId) != null)
                    queryImport = queryImport.Where(x => x.ProductId == reportFilter.ProductId);
            if (reportFilter.TypeId != null)
                if (_context.Types.Find(reportFilter.TypeId) != null)
                    queryImport = queryImport.Where(x => x.Product.ProductTypeId == reportFilter.TypeId);
            if (reportFilter.UserId != null)
                if (_context.Users.Find(reportFilter.UserId) != null)
                    queryImport = queryImport.Where(x => x.UserId == reportFilter.UserId);
            queryImport = queryImport.Where(pm => pm.Date.Date >= reportFilter.DateFrom.Date && pm.Date.Date <= reportFilter.DateTo.Date);
            reportFilter.ProductManagers = queryImport;
            ViewBag.paged = new PagedList<ProductManager>(queryImport, reportFilter.Page, reportFilter.PageSize);
        }
        [NonAction]
        void Saled(ReportViewModel reportFilter)
        {
            var querySaled = _context.ProductOrders.Include(po => po.Product.ProductType)
                                                       .Include(po => po.Order.User)
                                                       .Include(po => po.Order).AsQueryable();
            if (reportFilter.ProductId != null)
                querySaled = querySaled.Where(po => po.ProductId == reportFilter.ProductId);
            if (reportFilter.TypeId != null)
                querySaled = querySaled.Where(po => po.Product.ProductTypeId == reportFilter.TypeId);
            if (reportFilter.UserId != null)
                querySaled = querySaled.Where(po => po.Order.UserId == reportFilter.UserId);
            querySaled = querySaled.Where(po => po.Order.Date >= reportFilter.DateFrom.Date && po.Order.Date.Date <= reportFilter.DateTo.Date);
            reportFilter.ProductOrders = querySaled;
            ViewBag.paged = new PagedList<ProductOrder>(querySaled, reportFilter.Page, reportFilter.PageSize);
        }
        [NonAction]
        void Export(ReportViewModel reportFilter)
        {
            var queryExport = _context.WriteOuts.Include(wo => wo.Product.ProductType)
                                                  .Include(wo => wo.User)
                                                  .Include(wo => wo.Warehouse).AsQueryable();
            if (reportFilter.ProductId != null)
                queryExport = queryExport.Where(qe => qe.ProductId == reportFilter.ProductId);
            if (reportFilter.TypeId != null)
                queryExport = queryExport.Where(qe => qe.Product.ProductTypeId == reportFilter.TypeId);
            if (reportFilter.UserId != null)
                queryExport = queryExport.Where(qe => qe.UserId == reportFilter.UserId);
            queryExport = queryExport.Where(qe => qe.Date.Date >= reportFilter.DateFrom.Date && qe.Date.Date <= reportFilter.DateTo.Date);
            reportFilter.WriteOuts = queryExport;
            ViewBag.paged = new PagedList<WriteOut>(queryExport, reportFilter.Page, reportFilter.PageSize);
        }
        public IActionResult ExcelExport(ReportViewModel reportFilter)
        {
            if (!FilterValid()) return BadRequest();
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
            if (reportFilter.Deal.Equals("0"))
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
                queryImport = queryImport.Where(pm => pm.Date.Date >= reportFilter.DateFrom.Date && pm.Date.Date <= reportFilter.DateTo.Date);
                reportFilter.ProductManagers = queryImport;
                return ExportProductManager(queryImport.ToList());
            }
            if (reportFilter.Deal.Equals("1"))
            {
                var querySaled = _context.ProductOrders.Include(po => po.Product.ProductType)
                                                       .Include(po => po.Order.User).AsQueryable();
                if (reportFilter.ProductId != null)
                    querySaled = querySaled.Where(po => po.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    querySaled = querySaled.Where(po => po.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    querySaled = querySaled.Where(po => po.Order.UserId == reportFilter.UserId);
                querySaled = querySaled.Where(po => po.Order.Date.Date >= reportFilter.DateFrom.Date && po.Order.Date.Date <= reportFilter.DateTo.Date);
                reportFilter.ProductOrders = querySaled;
                return ExportProductOrder(querySaled.ToList());
            }
            if (reportFilter.Deal.Equals("2"))
            {
                var queryExport = _context.WriteOuts.Include(wo => wo.Product.ProductType)
                                                  .Include(wo => wo.User)
                                                  .Include(wo => wo.Warehouse).AsQueryable();
                if (reportFilter.ProductId != null)
                    queryExport = queryExport.Where(qe => qe.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryExport = queryExport.Where(qe => qe.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryExport = queryExport.Where(qe => qe.UserId == reportFilter.UserId);
                queryExport = queryExport.Where(qe => qe.Date.Date >= reportFilter.DateFrom.Date && qe.Date.Date <= reportFilter.DateTo.Date);
                reportFilter.WriteOuts = queryExport;
                return ExportWriteOut(queryExport.ToList());
            }
            return RedirectToAction("Index", new Dictionary<string, ReportViewModel> { { "reportFilter", reportFilter } });

        }
        [NonAction]
        FileStreamResult ExportProductManager(IList<ProductManager> list)
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
        [NonAction]
        FileStreamResult ExportProductOrder(IList<ProductOrder> list)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
        [NonAction]
        FileStreamResult ExportWriteOut(IList<WriteOut> list)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loading");
                sheet.Cells.LoadFromCollection(list.Select(po => new {
                    ProductName = po.Product.Name,
                    ProductType = po.Product.ProductType.Name,
                    Count=po.Count,
                    Price=po.Price,
                    WriteOutDate = po.Date.ToString("d"),
                    UserName = po.User.Name
                }), true);
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"ExportReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [NonAction]
        private bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                var keys = Request.Query.Keys;
                DateTime date=DateTime.MinValue;
                if(keys.Contains("PageSize"))
                    if (!byte.TryParse(Request.Query["PageSize"], out byte size) && size>0 && size<101) return false;
                if(keys.Contains("DateTo"))
                    if (!DateTime.TryParse(Request.Query["DateTo"], out date)) return false;
                if(keys.Contains("DateFrom"))
                    if (!DateTime.TryParse(Request.Query["DateFrom"], out date)) return false;
                if (keys.Contains("Page"))
                    if (!(uint.TryParse(Request.Query["Page"], out uint page) && page>0) ) return false;
            }
            return true;
        }
    }
}
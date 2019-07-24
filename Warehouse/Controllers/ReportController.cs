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
        /// Shows a report on received, withdrawn, sold and moved products
        /// </summary>
        /// <param name="reportFilter">Filtering data</param>
        /// <returns></returns>
        public IActionResult Index(IDictionary<string, string> data)
        {
            
            var a = Request.QueryString.ToString().Split('&').Where(s => s.Contains("WarehouseId")).Select(s => s.Substring(s.IndexOf('=') + 1)).ToList();
            data.Remove("WarehouseId");
            for(int i=0; i<a.Count(); i++)
            {
                data.Add($"Warehouse{i}", a[i]);
            }
            if (!FilterValid()) return BadRequest();
            ReportViewModel reportFilter = new ReportViewModel(data);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //var wareHouseId = user.WarehouseId;
            ViewBag.Names = new SelectList(_context.Products, "Id", "Name");
            ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users.Where(u=>u.Id!=user.Id),"Id","UserName");
            ViewBag.Warehouses = new SelectList(_context.Warehouses, "Id", "Number");
            if (reportFilter.Deal == null)
            {
                if (reportFilter.DateFrom == DateTime.MinValue)
                    reportFilter.DateFrom = DateTime.MinValue.Date;
                return View(reportFilter);
            }
            if (reportFilter.Deal.Equals("0"))
                Import(reportFilter);
            else if (reportFilter.Deal.Equals("1"))
                Saled(reportFilter);
            else if (reportFilter.Deal.Equals("2"))
                Export(reportFilter);
            else if (reportFilter.Deal.Equals("3"))
                Moved(reportFilter);
            else return BadRequest();
            return View(reportFilter);
        }

        [NonAction]
        void Import(ReportViewModel reportFilter)
        {
            if (_context.ProductManagers.Count() != 0)
            {
                ViewBag.House = reportFilter.WarehouseId;
                var queryImport = _context.ProductManagers
                                                   .Include(pm => pm.Product.ProductType)
                                                   .Include(pm => pm.User).AsQueryable();
                reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? queryImport.Min(pm => pm.Date).Date
                                                : reportFilter.DateFrom.Date;
                if (reportFilter.ProductId != null)
                    if (_context.Products.Find(reportFilter.ProductId) != null)
                        queryImport = queryImport.Where(x => x.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    if (_context.Types.Find(reportFilter.TypeId) != null)
                        queryImport = queryImport.Where(x => x.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    if (_context.Users.Find(reportFilter.UserId) != null)
                        queryImport = queryImport.Where(x => x.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    queryImport = queryImport.Where(pm => reportFilter.WarehouseId.Contains(pm.WareHouseId));
                queryImport = queryImport.Where(pm => pm.Date.Date >= reportFilter.DateFrom.Date && pm.Date.Date <= reportFilter.DateTo.Date);
                ViewBag.paged = new PagedList<ProductManager>(queryImport, reportFilter.page, reportFilter.pageSize);
            }
            else {
                ViewBag.paged = null;
                reportFilter.DateFrom = DateTime.MinValue.Date;
            } 
        }
        [NonAction]
        void Saled(ReportViewModel reportFilter)
        {
            if (_context.ProductOrders.Count() != 0)
            {
                var querySaled = _context.ProductOrders.Include(po => po.Product.ProductType)
                                                       .Include(po => po.Order.User)
                                                       .Include(po => po.Order).AsQueryable();
                reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? querySaled.Min(po => po.Order.Date)
                                                : reportFilter.DateFrom;
                if (reportFilter.ProductId != null)
                    querySaled = querySaled.Where(po => po.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    querySaled = querySaled.Where(po => po.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    querySaled = querySaled.Where(po => po.Order.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    querySaled = querySaled.Where(po => reportFilter.WarehouseId.Contains(po.Order.WareHouseId));
                querySaled = querySaled.Where(po => po.Order.Date >= reportFilter.DateFrom.Date && po.Order.Date.Date <= reportFilter.DateTo.Date);
                ViewBag.paged = new PagedList<ProductOrder>(querySaled, reportFilter.page, reportFilter.pageSize);
            }
            else {
                ViewBag.paged = null;
                reportFilter.DateFrom = DateTime.MinValue.Date;
            } 
        }
        [NonAction]
        void Moved(ReportViewModel reportFilter)
        {
            if (_context.ProductMoves.Count() != 0)
            {
                var productMove = _context.ProductMoves.Include(pm => pm.User)
                                                 .Include(pm => pm.Before)
                                                 .Include(pm => pm.After)
                                                 .Include(pm => pm.Product.ProductType)
                                                 .AsQueryable();
                reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? productMove.Min(pm=>pm.Date)
                                                : reportFilter.DateFrom;
                if (reportFilter.ProductId != null)
                    productMove = productMove.Where(pm => pm.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    productMove = productMove.Where(pm => pm.TypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    productMove = productMove.Where(pm => pm.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    productMove = productMove.Where(pm => reportFilter.WarehouseId.Contains(pm.Before.Id));
                productMove = productMove.Where(pm => pm.Date.Date >= reportFilter.DateFrom.Date && pm.Date.Date <= reportFilter.DateTo.Date);
                ViewBag.paged = new PagedList<ProductMove>(productMove, reportFilter.page, reportFilter.pageSize);
            }
            else
            {
                ViewBag.paged = null;
                reportFilter.DateFrom = DateTime.MinValue.Date;
            }

        }
        [NonAction]
        void Export(ReportViewModel reportFilter)
        {
            if (_context.WriteOuts.Count() != 0)
            {
                var queryExport = _context.WriteOuts.Include(wo => wo.Product.ProductType)
                                                  .Include(wo => wo.User)
                                                  .Include(wo => wo.Warehouse).AsQueryable();
                reportFilter.DateFrom = reportFilter.DateFrom == DateTime.MinValue
                                                ? queryExport.Min(wo => wo.Date).Date
                                                : new DateTime(2000,01,01);
                if (reportFilter.ProductId != null)
                    queryExport = queryExport.Where(qe => qe.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryExport = queryExport.Where(qe => qe.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryExport = queryExport.Where(qe => qe.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    queryExport = queryExport.Where(wo => reportFilter.WarehouseId.Contains(wo.WarehouseId));
                queryExport = queryExport.Where(qe => qe.Date.Date >= reportFilter.DateFrom.Date && qe.Date.Date <= reportFilter.DateTo.Date);
                ViewBag.paged = new PagedList<WriteOut>(queryExport, reportFilter.page, reportFilter.pageSize);
            }
            else
            {
                ViewBag.paged = null;
                reportFilter.DateFrom = DateTime.MinValue.Date;
            }
        }
        public IActionResult ExcelExport(IDictionary<string, string> data)
        {
            var a = Request.QueryString.ToString().Split('&').Where(s => s.Contains("WarehouseId")).Select(s => s.Substring(s.IndexOf('=') + 1)).ToList();
            data.Remove("WarehouseId");
            for (int i = 0; i < a.Count(); i++)
            {
                data.Add($"Warehouse{i}", a[i]);
            }
            if (!FilterValid()) return BadRequest();
            ReportViewModel reportFilter = new ReportViewModel(data);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
           // var wareHouseId = user.WarehouseId;
            ViewBag.Names = new SelectList(_context.Products, "Id", "Name");
            ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users/*.Where(u => u.WarehouseId == wareHouseId)*/, "Id", "UserName");
            if (reportFilter.Deal == null)
            {
                return RedirectToAction("Index", new Dictionary<string,string>());
            }
            if (reportFilter.Deal.Equals("0"))
            {
                var queryImport = _context.ProductManagers//.Where(pm => pm.WareHouseId == wareHouseId)
                                                .Include(pm => pm.Product.ProductType)
                                                .Include(pm => pm.User).AsQueryable();
                if (reportFilter.ProductId != null)
                    queryImport = queryImport.Where(x => x.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryImport = queryImport.Where(x => x.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryImport = queryImport.Where(x => x.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    queryImport = queryImport.Where(pm => reportFilter.WarehouseId.Contains(pm.WareHouseId));
                queryImport = queryImport.Where(pm => pm.Date.Date >= reportFilter.DateFrom.Date && pm.Date.Date <= reportFilter.DateTo.Date);
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
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    querySaled = querySaled.Where(po => reportFilter.WarehouseId.Contains(po.Order.WareHouseId));
                querySaled = querySaled.Where(po => po.Order.Date.Date >= reportFilter.DateFrom.Date && po.Order.Date.Date <= reportFilter.DateTo.Date);
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
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    queryExport = queryExport.Where(wo => reportFilter.WarehouseId.Contains(wo.WarehouseId));
                queryExport = queryExport.Where(qe => qe.Date.Date >= reportFilter.DateFrom.Date && qe.Date.Date <= reportFilter.DateTo.Date);
                return ExportWriteOut(queryExport.ToList());
            }
            if (reportFilter.Deal.Equals("3"))
            {
                var queryMoved = _context.ProductMoves.Include(pm=>pm.Product.ProductType)
                                                  .Include(pm => pm.User)
                                                  .Include(pm=>pm.After)
                                                  .Include(pm=>pm.Before).AsQueryable();
                if (reportFilter.ProductId != null)
                    queryMoved = queryMoved.Where(qe => qe.ProductId == reportFilter.ProductId);
                if (reportFilter.TypeId != null)
                    queryMoved = queryMoved.Where(qe => qe.Product.ProductTypeId == reportFilter.TypeId);
                if (reportFilter.UserId != null)
                    queryMoved = queryMoved.Where(qe => qe.UserId == reportFilter.UserId);
                if (reportFilter.WarehouseId != null && reportFilter.WarehouseId.Count != 0)
                    queryMoved = queryMoved.Where(pm => reportFilter.WarehouseId.Contains(pm.Before.Id));
                queryMoved = queryMoved.Where(qe => qe.Date.Date >= reportFilter.DateFrom.Date && qe.Date.Date <= reportFilter.DateTo.Date);
                return ExportMoved(queryMoved.ToList());
            }
            return RedirectToAction("Index", new Dictionary<string, string>());

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
        FileStreamResult ExportMoved(IList<ProductMove> list)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loading");
                sheet.Cells.LoadFromCollection(list.Select(pm => new {
                    ProductName = pm.Product.Name,
                    ProductType = pm.Product.ProductType.Name,
                    Count = pm.Count,
                    MovedDate = pm.Date.ToString("d"),
                    UserName = pm.User.Name
                }), true);
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"MovedReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [NonAction]
        private bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                var keys = Request.Query.Keys;
                DateTime date=DateTime.MinValue;
                if(keys.Contains("pageSize"))
                    if (!(byte.TryParse(Request.Query["PageSize"], out byte size) && size>0 && size<101)) return false;
                if(keys.Contains("DateTo"))
                    if (!DateTime.TryParse(Request.Query["DateTo"], out date)) return false;
                if(keys.Contains("DateFrom"))
                    if (!DateTime.TryParse(Request.Query["DateFrom"], out date)) return false;
                if (keys.Contains("page"))
                    if (!(uint.TryParse(Request.Query["page"], out uint page) && page>0) ) return false;
            }
            return true;
        }
    }
}
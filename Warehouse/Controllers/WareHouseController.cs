using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WareHouseController : Controller
    {
        readonly ILogger<WareHouseController> _log;

        private readonly ApplicationDbContext _context;
        public WareHouseController(ApplicationDbContext context, ILogger<WareHouseController> log,
        ILogger<WareHouseController> logger)
        {
            _log = logger;
            _context = context;
        }

        public IActionResult Index(string number, string address, int page = 1, int pageSize = 10)
        {
            if (!FilterValid()) return BadRequest();
            var query = _context.Warehouses.AsQueryable().OrderBy(d => d.CreationDate);
            if (!string.IsNullOrEmpty(number))
                query = query.Where(w => w.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase)).OrderBy(d => d.CreationDate);
            if (!string.IsNullOrEmpty(address))
                query = query.Where(w => w.Address.Contains(address, StringComparison.InvariantCultureIgnoreCase)).OrderBy(d=>d.CreationDate);
            ViewData["CurrentNumber"] = number;
            ViewData["CurrentAddress"] = address;
            ViewData["CurrentSize"] = pageSize;
            PagedList<WareHouse> model = new PagedList<WareHouse>(query, page, pageSize);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Warehouse index. "+user);
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(WareHouse wh)
        {
            if (_context.Warehouses.FirstOrDefault(w => w.Number == wh.Number) != null)
                ModelState.AddModelError("", "The number of warehouse already exists");
            if (ModelState.IsValid)
            {
                wh.IsActive = true;
                wh.CreationDate = DateTime.Now;
                _context.Add(wh);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Created warehouse.User: "+user);
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(string id)
        {
            var obj = _context.Warehouses.Find(id);
            if (obj == null) return BadRequest();
            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(WareHouse wh)
        {
            if (_context.Warehouses.FirstOrDefault(w => w.Number == wh.Number && w.Id != wh.Id) != null)
                ModelState.AddModelError("", "The number of warehouse already exists");
            if (ModelState.IsValid)
            {
                _context.Warehouses.Update(wh);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Edited warehouse.User: "+user);
                return RedirectToAction("Index");
            }
            return View();
        }
        

        
        [HttpPost]
        public JsonResult Disable([FromBody]string warehouseId)
        {
            var data = _context.Warehouses.Find(warehouseId);
            if (data == null || !data.IsActive) return Json(false);
            var productManagers = _context.ProductManagers.Where(pm => pm.WareHouseId == warehouseId);
            foreach(var item in productManagers)
            {
                if (item.CurrentCount != 0) return Json(false);
            }
            data.IsActive = false;
            _context.Warehouses.Update(data);
            _context.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult Enable([FromBody]string warehouseId)
        {
            var data = _context.Warehouses.Find(warehouseId);
            if (data == null || data.IsActive) return Json(false);
            data.IsActive = true;
            _context.Update(data);
            _context.SaveChanges();
            return Json(true);
        }
        [NonAction]
        bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                var keys = Request.Query.Keys;
                var request = Request.Query;
                if (keys.Contains("PageSize"))
                    if (!(byte.TryParse(request["PageSize"], out byte size) && size > 0 && size < 101)) return false;
                if (keys.Contains("Page"))
                    if (!(uint.TryParse(request["Page"], out uint page) && page > 0)) return false;
            }
            return true;
        }
    }
}
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
        private readonly IStringLocalizer<WareHouseController> _localizer;
        private readonly ILogger<WareHouseController> _logger;

        private readonly ApplicationDbContext _context;
        public WareHouseController(ApplicationDbContext context, IStringLocalizer<WareHouseController> localizer,
    ILogger<WareHouseController> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        public IActionResult Index(string number, string address, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation(_localizer["Product"]);
            number = number == null ? "" : number.Trim();
            address = address == null ? "" : address.Trim();

            IQueryable<WareHouse> wareHouses = _context.Warehouses.Where(w => w.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase) && w.Address.Contains(address, StringComparison.InvariantCultureIgnoreCase));

            ViewData["CurrentNumber"] = number;
            ViewData["CurrentAddress"] = address;
            ViewData["CurrentSize"] = pageSize;

            PagedList<WareHouse> model = new PagedList<WareHouse>(wareHouses, page, pageSize);
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(WareHouse wh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wh);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(string id)
        {
            var obj = _context.Warehouses.Find(id);
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(WareHouse wh)
        {
            if (ModelState.IsValid)
            {
                _context.Warehouses.Update(wh);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(string id)
        {
            return View(_context.Warehouses.Find(id));
        }
        public IActionResult DeleteYes(string id)
        {
            _context.Warehouses.Remove(_context.Warehouses.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult Details(string id)
        {
            var obj = _context.Warehouses.Find(id);
            return View(obj);
        }
    }
}
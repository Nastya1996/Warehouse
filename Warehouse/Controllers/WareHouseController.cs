using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class WareHouseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WareHouseController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var whDatas = _context.Warehouses;
            return View(whDatas.ToList());
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
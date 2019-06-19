using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class UnitController : Controller
    {
        readonly ILogger<UnitController> _log;
        private readonly ApplicationDbContext _context;
        
        public UnitController(ApplicationDbContext context, ILogger<UnitController> log)
        {
            _log = log;
            _context = context;
        }


        public IActionResult Index()
        {
            var unitDatas = _context.Units;
            _log.LogInformation("Unit index.");
            return View(unitDatas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Unit unit)
        {
            if (ModelState.IsValid) {
                _context.Add(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            _log.LogInformation("Unit create.");
            return View();
        }
        public IActionResult Edit(string id)
        {
            var obj = _context.Units.Find(id);
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Units.Update(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            _log.LogInformation("Edit index.");
            return View();
        }
        public IActionResult Delete(string id)
        {
            return View(_context.Units.Find(id));
        }
        public IActionResult DeleteYes(string id)
        {
            _context.Units.Remove(_context.Units.Find(id));
            _context.SaveChanges();
            _log.LogInformation("Unit delete.");
            return RedirectToAction("Index");
        }
        public IActionResult Details(string id)
        {
            var obj = _context.Units.Find(id);
            _log.LogInformation("Unit details.");
            return View(obj);
            //return View(_context.Files.ToList());
        }
    }
}
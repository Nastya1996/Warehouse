using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PagedList.Core;
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


        //public IActionResult Index()
        //{
        //    var unitDatas = _context.Units;
        //    var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    _log.LogInformation("Unit index."+user);
        //    return View(unitDatas.ToList());
        //}
        public IActionResult Index(string name, int page=1, int pageSize = 10)
        {
            if (!FilterValid()) return BadRequest();
            ViewData["CurrentName"] = name;
            ViewData["CurrentSize"] = pageSize;
            var query = _context.Units.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            PagedList<Unit> model = new PagedList<Unit>(query, page, pageSize);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Unit index."+user);
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Unit unit)
        {
            if (_context.Units.FirstOrDefault(u => u.Name == unit.Name) != null)
                ModelState.AddModelError("", "This name of unit is available in the database");
            if (ModelState.IsValid)
            {
                _context.Add(unit);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Unit createed."+user);
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(string id)
        {
            var obj = _context.Units.Find(id);
            if (obj == null) return BadRequest();
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Unit unit)
        {
            if (_context.Units.FirstOrDefault(u => u.Name == unit.Name && u.Id != unit.Id) != null)
                ModelState.AddModelError("", "This name of unit is available in the database");
            if (ModelState.IsValid)
            {
                _context.Units.Update(unit);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Edited index."+user);
                return RedirectToAction("Index");
            }
            return View();
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
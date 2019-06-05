using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Models;
using Warehouse.Data;
using Microsoft.AspNetCore.Authorization;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Types.ToList());
        }


        //Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                _context.Types.Add(productType);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return Create();
        }


        //Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View(_context.Types.FirstOrDefault(x => x.Id == id));
        }
        [HttpPost]
        public IActionResult Edit(ProductType productType)
        {
            _context.Update(productType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Delete
        [HttpGet]
        public IActionResult Delete(string id)
        {
            return View(_context.Types.FirstOrDefault(x => x.Id == id));
        }
        [HttpGet]
        public IActionResult Deleted(string id)
        {
            var obj = _context.Types.Find(id);
            if (obj == null) return NotFound();
            _context.Remove(_context.Types.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            return View(_context.Types.FirstOrDefault(x=>x.Id==id));
        }
        //Type availability
        public JsonResult TypeAvailability(string Name)
        {
            Name = Name.Trim();
            if (_context.Types.FirstOrDefault(pt => pt.Name == Name)!=null)
                return Json("*This type of product is available in the database");
            return Json(true);
        }
    }
}
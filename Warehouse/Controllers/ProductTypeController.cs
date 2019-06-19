using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Models;
using Warehouse.Data;
using Microsoft.AspNetCore.Authorization;
using PagedList.Core;
using Microsoft.Extensions.Logging;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly ILogger<ProductTypeController> _log;
        public ProductTypeController(ApplicationDbContext context, ILogger<ProductTypeController> log)
        {
            _log = log;
            _context = context;
        }

        public IActionResult Index(string type, int page=1, int pageSize=10)
        {
            type = type == null ? "" : type.Trim();
            ViewData["CurrentSize"] = pageSize;
            ViewData["CurrentType"] = type;
            var types = _context.Types.Where(t => t.Name.Contains(type,StringComparison.InvariantCultureIgnoreCase)).AsQueryable();
            PagedList<ProductType> model = new PagedList<ProductType>(types, page,pageSize);
            _log.LogInformation("Product type index.");
            return View(model);
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
            if ((_context.Types.FirstOrDefault(pt => pt.Name == productType.Name)) != null)
            {
                ModelState.AddModelError("","This type of product is available in the database");
            }
            if (ModelState.IsValid)
            {
                _context.Types.Add(productType);
                _context.SaveChanges();
                _log.LogInformation("Product type create.");
                return RedirectToAction("Index");
            }
            return View(productType);
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
            if ((_context.Types.FirstOrDefault(pt => pt.Name == productType.Name && pt.Id != productType.Id)) != null)
            {
                ModelState.AddModelError("", "This type of product is available in the database");
            }
            if (ModelState.IsValid)
            {
                _context.Update(productType);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            _log.LogInformation("Product type edit.");
            return View(productType);
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
            _log.LogInformation("Product type delete.");
            return RedirectToAction("Index");
        }


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            _log.LogInformation("Product type details.");
            return View(_context.Types.FirstOrDefault(x=>x.Id==id));
        }
    }
}
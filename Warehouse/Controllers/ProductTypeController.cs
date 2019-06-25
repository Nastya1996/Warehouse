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
using System.Security.Claims;

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


        /// <summary>
        /// Show product types
        /// </summary>
        /// <param name="type">Product type</param>
        /// <param name="page">Current page. Default 1</param>
        /// <param name="pageSize">Page size. Default 10</param>
        /// <returns>Product types</returns>
        public IActionResult Index(string type, int page=1, int pageSize=10)
        {
            type = type == null ? "" : type.Trim();
            ViewData["CurrentSize"] = pageSize;
            ViewData["CurrentType"] = type;
            var types = _context.Types.Where(t => t.Name.Contains(type,StringComparison.InvariantCultureIgnoreCase)).AsQueryable();
            PagedList<ProductType> model = new PagedList<ProductType>(types, page,pageSize);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product type index.User: "+user);
            return View(model);
        }


        /// <summary>
        /// Open product type creation window
        /// </summary>
        /// <returns>Product type creation window</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// Add new product type
        /// </summary>
        /// <param name="productType">ProductType type object</param>
        /// <returns>Product types</returns>
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
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product type created.User: "+user);
                return RedirectToAction("Index");
            }
            return View(productType);
        }


        /// <summary>
        /// Open product edition window
        /// </summary>
        /// <param name="id">Product type id</param>
        /// <returns>Product edition window</returns>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View(_context.Types.FirstOrDefault(x => x.Id == id));
        }


        /// <summary>
        /// Edit product type
        /// </summary>
        /// <param name="productType">ProductType type object</param>
        /// <returns>Show product types</returns>
        [HttpPost]
        public IActionResult Edit(ProductType productType)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if ((_context.Types.FirstOrDefault(pt => pt.Name == productType.Name && pt.Id != productType.Id)) != null)
            {
                ModelState.AddModelError("", "This type of product is available in the database");
            }
            if (ModelState.IsValid)
            {
                _context.Update(productType);
                _context.SaveChanges();
                _log.LogInformation("Product type edited.User: "+user);
                return RedirectToAction("Index");
            }
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
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var obj = _context.Types.Find(id);
            if (obj == null) return NotFound();
            _context.Remove(_context.Types.Find(id));
            _context.SaveChanges();
            _log.LogInformation("Product type deleted.User: "+user);
            return RedirectToAction("Index");
        }


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product type details.User: "+user);
            return View(_context.Types.FirstOrDefault(x=>x.Id==id));
        }
    }
}
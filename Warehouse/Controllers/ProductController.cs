using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper, Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context) => _context = context;
        public async Task<IActionResult> Index(SortState sortOrder = SortState.ProductNameAsc)
        {
            IQueryable<Product> products = _context.Products.Include(x => x.ProductType).Include(x => x.Unit);

            ViewBag.ProductNameSort = sortOrder == SortState.ProductNameAsc ? SortState.ProductNameDesc : SortState.ProductNameAsc;

            switch (sortOrder)
            {
                case SortState.ProductNameDesc:
                    products = products.OrderByDescending(s => s.Name);
                    break;
            }

            return View(await products.AsNoTracking().ToListAsync());
        }


        //Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ProductTypes = new SelectList(_context.Types,"Id", "Name");
            ViewBag.Units = new SelectList(_context.Units, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.IsActive = true;
                _context.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        

        //Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Units = new SelectList(_context.Units, "Id", "Name");
            return View(_context.Products.Include(x => x.ProductType).Include(x => x.Unit).FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            _context.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            return View(_context.Products.Include(x=>x.ProductType).Include(x=>x.Unit).FirstOrDefault(x => x.Id == id));
        }

        //Disable product
        [HttpPost]
        [Route("Products/Disable/")]
        public JsonResult Disable([FromBody]string productId)
        {
            var product = _context.Products.Find(productId);
            if(product == null)
                return Json(false);
            else if(product.IsActive==true)
            {
                product.IsActive = false;
                _context.Update(product);
                _context.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
        //Enable product
        [HttpPost]
        [Route("Products/Enable/")]
        public JsonResult Enable([FromBody]string productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return Json(false);
            else if (product.IsActive == false)
            {
                product.IsActive = true;
                _context.Update(product);
                _context.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
        
        //Product Availability
        public JsonResult ProductAvailability(string Name)
        {
            Name = Name.Trim();
            if (_context.Products.FirstOrDefault(p => p.Name == Name) != null)
                return Json("*The name of product is available in the database");
            return Json(true);
        }
        
    }
}
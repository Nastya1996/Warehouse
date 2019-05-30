using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
namespace Warehouse.Controllers
{
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
            product.IsActive = true; 
            _context.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Delete
        [HttpGet]
        public IActionResult Delete(string id)
        {
            return View(_context.Products.FirstOrDefault(x => x.Id == id));
        }
        [HttpGet]
        public IActionResult DeleteProduct(string id)
        {
            var data = _context.Products.Find(id);
            if (data == null) return NotFound();
            _context.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Models;
namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper, Admin")]
    public class ProductController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context) => _context = context;
        public IActionResult Index(string name, string type, SortState sortOrder = SortState.ProductNameAsc, int page = 1, int pageSize = 10)
        {
            name = name == null ? "" : name.Trim();
            type = type == null ? "" : type.Trim();

            IQueryable<Product> products = _context.Products.Where(p=>p.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase) && p.ProductType.Name.Contains(type, StringComparison.InvariantCultureIgnoreCase)).Include(x => x.ProductType).Include(x => x.Unit);
            ViewBag.ProductNameSort = sortOrder == SortState.ProductNameAsc ? SortState.ProductNameDesc : SortState.ProductNameAsc;
            switch (sortOrder)
            {
                case SortState.ProductNameDesc:
                    products = products.OrderByDescending(s => s.Name);
                    break;
            }

            ViewData["CurrentName"] = name;
            ViewData["CurrentType"] = type;
            ViewData["CurrentSize"] = pageSize;
            PagedList<Product> model = new PagedList<Product>(products, page, pageSize);

            return View(model);
        }


        //Create
        [HttpGet]
        public IActionResult Create()
        {
            SelectInitial();
            return View();
        }


        [HttpPost]
        public IActionResult Create(Product product)
        {
            SelectInitial();
            if (string.IsNullOrEmpty(product.ProductTypeId))
                ModelState.AddModelError("","The product type not selected");
            if (string.IsNullOrEmpty(product.UnitId))
                ModelState.AddModelError("", "The unit not selected");
            if (_context.Products.FirstOrDefault(p => p.Name == product.Name) != null)
                ModelState.AddModelError("", "This name of product is available in the database");
            if (ModelState.IsValid)
            {
                product.IsActive = true;
                _context.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }


        //Initial Select tags
        void SelectInitial()
        {
            ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Units = new SelectList(_context.Units, "Id", "Name");
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
            SelectInitial();
            if((_context.Products.FirstOrDefault(p=>p.Name==product.Name && p.Id != product.Id))!=null)
                ModelState.AddModelError("", "This name of product is available in the database");
            if (ModelState.IsValid)
            {
                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
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

        //Get Products
        [HttpPost]
        [Route("Products/Get")]
        public JsonResult GetProduct([FromBody]string selected)
        {
            return Json(_context.Products.Where(p => p.ProductTypeId == selected && p.IsActive != false).ToList());
        }

    }
}
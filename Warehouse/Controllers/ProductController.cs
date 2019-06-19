using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private IHostingEnvironment _appEnvironment;

        public ProductController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public IActionResult Index(string name, string type, SortState sortOrder = SortState.ProductNameAsc, int page = 1, int pageSize = 10)
        {
            name = name == null ? "" : name.Trim();
            type = type == null ? "" : type.Trim();
            IQueryable<Product> products = _context.Products.Where(p=>p.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase) && p.ProductType.Name.Contains(type, StringComparison.InvariantCultureIgnoreCase)).Include(x => x.ProductType).Include(x => x.Unit).Include(f => f.FileModelImg);
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
        public async Task<IActionResult> Create(Product product, IFormFile uploadedFile)
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
                var imgID = "";
                if (uploadedFile != null)
                {
                    // путь к папке Files
                    string path = "/Files/" + uploadedFile.FileName;
                    // сохраняем файл в папку Files в каталоге wwwroot
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    FileModelImg file = new FileModelImg { Name = uploadedFile.FileName, Path = path };
                    _context.Files.Add(file);
                    _context.SaveChanges();
                    //imgID = _context.Files.Find(file).Id;
                    product.FileModelImg = file;
                }
                
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
        
    }
}
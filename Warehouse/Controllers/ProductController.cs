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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Infrastructure;
using Warehouse.Models;
namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper, Admin")]
    public class ProductController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _appEnvironment;
        readonly ILogger<ProductController> _log;

        public ProductController(ApplicationDbContext context, IHostingEnvironment appEnvironment, ILogger<ProductController> log)
        {
            _log = log;
            _context = context;
            _appEnvironment = appEnvironment;
        }


        /// <summary>
        /// Show products
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="type">Product type</param>
        /// <param name="sortOrder">Sorting type</param>
        /// <param name="page">Current page. Default 1</param>
        /// <param name="pageSize">Page size. Default 10</param>
        /// <returns></returns>
        public IActionResult Index(ProductViewModel viewModel, SortState sortOrder = SortState.ProductNameAsc)
        {
            if (viewModel.PageSize < 1) {
                viewModel.PageSize = 10;
                return View(viewModel);
            }
            ViewBag.Types = new SelectList(_context.Types.Where(t => t.IsActive), "Id", "Name");
            ViewBag.Names = new SelectList(_context.Products.Where(p => p.IsActive), "Id", "Name");
            ViewData["CurrentSize"] = viewModel.PageSize;
            ViewBag.ProductNameSort = sortOrder == SortState.ProductNameAsc ? SortState.ProductNameDesc : SortState.ProductNameAsc;
            var query = _context.Products.Where(p => p.IsActive).AsQueryable();
            if (viewModel.TypeId != null)
                query = query.Where(p => p.ProductTypeId == viewModel.TypeId);
            if (viewModel.ProductId != null)
                query = query.Where(p => p.Id == viewModel.ProductId);
            switch (sortOrder)
            {
                case SortState.ProductNameDesc:
                    query = query.OrderByDescending(s => s.Name);
                    break;
            }
            ViewBag.paged = new PagedList<Product>(query, viewModel.Page, viewModel.PageSize);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product index.User: " + user);
            return View(viewModel);
        }



        /// <summary>
        /// Open product creation window
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            SelectInitial();
            return View();
        }



        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="product">Product type object</param>
        /// <param name="uploadedFile">Select filte to upload</param>
        /// <returns>Show products</returns>
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
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product created.User: "+user);
                return RedirectToAction("Index");
            }
            return View(product);
        }


        /// <summary>
        /// Initial the select tags
        /// </summary>
        void SelectInitial()
        {
            ViewBag.ProductTypes = new SelectList(_context.Types.Where(pt=>pt.IsActive), "Id", "Name");
            ViewBag.Units = new SelectList(_context.Units, "Id", "Name");
        }


        /// <summary>
        /// Open product edition window
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            SelectInitial();
            return View(_context.Products.Include(x => x.ProductType).Include(x => x.Unit).FirstOrDefault(x => x.Id == id));
        }



        /// <summary>
        /// Edit Product
        /// </summary>
        /// <param name="product">Product type object</param>
        /// <returns></returns>
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
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product edited.User: "+user);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        /// <summary>
        /// Show product details
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product details."+user);
            return View(_context.Products.Include(x=>x.ProductType).Include(x=>x.Unit).FirstOrDefault(x => x.Id == id));
        }



        /// <summary>
        /// Disable product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Disable product</returns>
        [HttpPost]
        [Route("Products/Disable/")]
        public JsonResult Disable([FromBody]string productId)
        {
            var product = _context.Products.Find(productId);
            if(product == null)
                return Json(false);
            else if(product.IsActive==true)
            {
                var productManager = _context.ProductManagers.Where(p => p.ProductId == productId);
                foreach(var item in productManager)
                    if (item.CurrentCount != 0)
                        return Json(false);
                product.IsActive = false;
                _context.Update(product);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product disabled."+user);
                return Json(true);
            }
            return Json(false);
        }


        /// <summary>
        /// Enable product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Enable product</returns>
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
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product enabled."+user);
                return Json(true);
            }
            return Json(false);
        }



        /// <summary>
        /// Receive products depending on the type selected
        /// </summary>
        /// <param name="selected">Product type</param>
        /// <returns>Products</returns>
        [HttpPost]
        [Route("Products/Get")]
        public JsonResult GetProduct([FromBody]string selected)
        {
            var products = _context.Products.AsQueryable();
            if (selected != "")
                products = products.Where(p => p.ProductTypeId == selected && p.IsActive);
            return Json(products.ToList());
        }

    }
}
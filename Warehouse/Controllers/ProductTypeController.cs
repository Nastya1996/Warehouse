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
using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper, Admin")]
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
        //public IActionResult Index(string type, int page=1, int pageSize=10)
        //{
        //    var query = _context.Types.AsQueryable();
        //    if (type != null)
        //    {
        //        type = type.Trim();
        //        query = query.Where(pt => pt.Name.Contains(type, StringComparison.InvariantCultureIgnoreCase));
        //    }
        //    ViewData["CurrentSize"] = pageSize;
        //    ViewData["CurrentType"] = type;
        //    PagedList<ProductType> model = new PagedList<ProductType>(query, page,pageSize);
        //    var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    _log.LogInformation("Product type index.User: "+user);
        //    return View(model);
        //}
        public IActionResult Index(ProductTypeViewModel viewModel)
        {
            if (viewModel.PageSize < 1) return NotFound();
            var query = _context.Types.AsQueryable();
            if (viewModel.TypeId != null)
                query = query.Where(pt => pt.Id == viewModel.TypeId);
            ViewData["CurrentSize"] = viewModel.PageSize;
            ViewBag.paged = new PagedList<ProductType>(query, viewModel.Page, viewModel.PageSize);
            ViewBag.Types = new SelectList(_context.Types.Where(pt => pt.IsActive),"Id","Name");
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product type index.User: "+user);
            return View(viewModel);
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
                productType.IsActive = true;
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
        
        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product type details.User: "+user);
            return View(_context.Types.FirstOrDefault(x=>x.Id==id));
        }

        [HttpPost]
        public JsonResult Enable([FromBody]string productTypeId)
        {
            var type = _context.Types.Find(productTypeId);
            if (type == null)
                return Json(false);
            else if (!type.IsActive)
            {
                var products = _context.Products.Where(p => p.ProductTypeId == productTypeId);
                foreach (var p in products)
                    p.IsActive = true;
                type.IsActive = true;
                _context.Update(type);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Product type enabled." + user);
                return Json(true);
            }
            return Json(false);
        }


        [HttpPost]
        public JsonResult Disable([FromBody]string productTypeId)
        {
            var type = _context.Types.Find(productTypeId);
            if (type==null)
                return Json(false);
            else
            {
                bool okey = true;
                var products = _context.Products.Where(p => p.ProductTypeId == productTypeId);
                foreach(var product in products)
                {
                    var productManager = _context.ProductManagers.Where(p => p.ProductId == product.Id);
                    foreach(var pmGroup in productManager)
                    {
                        if (pmGroup.CurrentCount != 0)
                        {
                            okey = false;
                            break;
                        }
                    }
                    if (!okey) return Json(false);
                }
                if (okey)
                {
                    type.IsActive = false;
                    foreach (var p in products)
                        p.IsActive = false;
                    _context.Update(type);
                    _context.SaveChanges();
                    var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _log.LogInformation("Product type and all products disabled. " + user);
                    return Json(true);
                }
            }
            return Json(false);
        }
    }
}
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
        ///// </summary>
        ///// <param name="type">Product type</param>
        ///// <param name="page">Current page. Default 1</param>
        ///// <param name="pageSize">Page size. Default 10</param>
        /// <returns>Product types</returns>
     
        public IActionResult Index(ProductTypeViewModel viewModel)
        {
            if (!FilterValid()) return BadRequest();
            IQueryable<ProductType> query = null;
            if (User.IsInRole("Admin"))
            {
                query = _context.Types.AsQueryable();
                ViewBag.Types = new SelectList(_context.Types, "Id", "Name");
            }
            else
            {
                query = _context.Types.Where(pt => pt.IsActive).AsQueryable();
                ViewBag.Types = new SelectList(_context.Types.Where(pt => pt.IsActive), "Id", "Name");
            }
            if (viewModel.TypeId != null)
                if (_context.Types.Find(viewModel.TypeId) != null)
                    query = query.Where(pt => pt.Id == viewModel.TypeId);
                else return BadRequest();
            ViewData["CurrentSize"] = viewModel.PageSize;
            ViewBag.paged = new PagedList<ProductType>(query, viewModel.Page, viewModel.PageSize);
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
        ///// <param name="productType">ProductType type object</param>
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
        ///// <param name="id">Product type id</param>
        /// <returns>Product edition window</returns>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var type = _context.Types.Find(id);
            if (type == null) return BadRequest();
            return View(type);
        }


        /// <summary>
        /// Edit product type
        /// </summary>
        ///// <param name="productType">ProductType type object</param>
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
                productType.IsActive = true;
                _context.Update(productType);
                _context.SaveChanges();
                _log.LogInformation("Product type edited.User: "+user);
                return RedirectToAction("Index");
            }
            return View(productType);
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
        [NonAction]
        bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                var keys = Request.Query.Keys;
                var request = Request.Query;
                if(keys.Contains("PageSize"))
                    if (!(byte.TryParse(request["PageSize"], out byte size) && size > 0 && size < 101)) return false;
                if(keys.Contains("Page"))
                    if (!(uint.TryParse(request["Page"], out uint page) && page > 0)) return false;
            }
            return true;
        }
    }
}
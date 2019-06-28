using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.HtmlHelper;
using Warehouse.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Warehouse.Infrastructure;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Storekeeper, Worker")]
    public class ProductManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly ILogger<ProductManagerController> _log;

        public ProductManagerController(ApplicationDbContext context, ILogger<ProductManagerController> log)
        {
            _log = log;
            _context = context;
        }


        ///// <summary>
        ///// Show product manager group by product
        ///// </summary>
        ///// <param name="type">Product type</param>
        ///// <param name="name">Product name</param>
        ///// <param name="page">Current page. Default page 1</param>
        ///// <param name="pageSize">Page size. Default size 10</param>
        /// <returns></returns>
        public IActionResult Index(ProductManagerViewModel viewModel)
        {
            if (!FilterValid()) return BadRequest();
            var user= _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var query = _context.ProductManagers
                .Where(pm => pm.WareHouseId == user.WarehouseId && pm.Product.IsActive &&
                 pm.Product.ProductType.IsActive)
                .Include(p => p.Product.ProductType)
                .Include(p => p.Product.Unit).AsQueryable();
            if (viewModel.TypeId != null)
                if(_context.Types.Find(viewModel.TypeId)!=null)
                    query = query.Where(pm => pm.Product.ProductTypeId == viewModel.TypeId);
            if (viewModel.ProductId != null)
                if(_context.Products.Find(viewModel.ProductId)!=null)
                    query = query.Where(pm => pm.ProductId == viewModel.ProductId);
            var queryGroup=query.GroupBy(pm=>pm.Product).Select(item => new
                       {
                           Product = item.Key,
                           Count = item.Sum(p => p.Count),
                           CurrentCount = item.Sum(p => p.CurrentCount)
                       }).Select(c=>c.ToExpando());
            ViewBag.paged = new PagedList<ExpandoObject>(queryGroup, viewModel.Page, viewModel.PageSize);
            ViewBag.Types = new SelectList(_context.Types.Where(pt => pt.IsActive), "Id", "Name");
            ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive), "Id", "Name");
            _log.LogInformation("Product manager index.User: " + user);
            return View(viewModel);
        }

        /// <summary>
        /// Open the create product manager window
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Storekeeper")]
        [HttpGet]
        public IActionResult Create()
        {
            SelectInitial();
            return View();
        }



        /// <summary>
        /// Create new productManager
        /// </summary>
        ///// <param name="productManager">ProductManager object</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(ProductManager productManager)
        {
            SelectInitial();
            if (string.IsNullOrEmpty(productManager.ProductId))
            {
                ModelState.AddModelError("","The product type or product name not selected");
            }
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var product = _context.Products.Include(u => u.Unit).FirstOrDefault(p => p.Id == productManager.ProductId);
                productManager.Date = DateTime.Now;
                productManager.UserId = user.Id;
                productManager.CurrentCount = productManager.Count;
                productManager.Product = product;
                productManager.WareHouseId = user.WarehouseId;
                _context.Add(productManager);
                _context.SaveChanges();
                _log.LogInformation("Product manager created.User: " + user);
                return RedirectToAction("Index");
            }
            return View(productManager);
        }



        /// <summary>
        /// Initial the select box
        /// </summary>
        void SelectInitial()
        {
            ViewBag.WareHouses = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name");
        }

        

        /// <summary>
        /// Show all product managers
        /// </summary>
        ///// <param name="id">Product id</param>
        ///// <param name="from">Date from</param>
        ///// <param name="before">Date before</param>
        ///// <param name="page">Current page. Default 1</param>
        ///// <param name="pageSize">Page size. Default 10</param>
        /// <returns></returns>
        public IActionResult Show(string id, decimal from, decimal before, int page=1, int pageSize=1)
        {
            if (!FilterValid()) return BadRequest();
            decimal data;
            if (before == 0)
            {
                data = _context.ProductManagers.Max(p => p.SalePrice);
            }
            else data = before;
            ViewData["CurrentId"] = id;
            ViewData["CurrentFrom"] = from;
            ViewData["CurrentBefore"] = before;
            ViewData["CurrentSize"] = pageSize;
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var maxPrice = _context.ProductManagers.Max(p => p.SalePrice);
            var minPrice = _context.ProductManagers.Min(p => p.SalePrice);
            var products = _context.ProductManagers
                .Where(p => p.ProductId == id && p.WareHouseId==user.WarehouseId &&
                 p.SalePrice>=from && p.SalePrice<=data)
                .Include(p=>p.Product)
                .Include(u=>u.Product.Unit)
                .Include(w=>w.WareHouse)
                .AsQueryable();
            PagedList<ProductManager> model = new PagedList<ProductManager>(products, page, pageSize);
            _log.LogInformation("Product manager show.User "+user);
            return View(model);
        }




        /// <summary>
        /// Open the edit ProductManager window
        /// </summary>
        ///// <param name="id">ProductManager Id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var productManager = _context.ProductManagers.Find(id.ToString());
            if(productManager!=null)
                return View(productManager);
            return RedirectToAction("Index");
        }

        ///// <summary>
        ///// Edit the ProductManager receipt and sale price
        ///// </summary>
        ///// <param name="productManager">ProductManager object</param>
        ///// <returns></returns>
        ///// 



        [HttpPost]
        public IActionResult Edit(ProductManager productManager)
        {
            var data= _context.ProductManagers.AsNoTracking().Where(p => p.ProductId == productManager.ProductId)
                .Include(p => p.Product).ToList();
            var prodManager = _context.ProductManagers.AsNoTracking()
                .FirstOrDefault(pm => pm.Id == productManager.Id);
            prodManager.ReceiptPrice = productManager.ReceiptPrice;
            prodManager.SalePrice = productManager.SalePrice;
            _context.Update(prodManager);
            _context.SaveChanges();
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product manager edited.User: "+user);
            return RedirectToAction("Show", new Dictionary<string, string> { { "id", prodManager.ProductId} });
        }



        ///// <summary>
        ///// Add products to the basket
        ///// </summary>
        ///// <param name="id">ProductManager Id</param>
        ///// <param name="quantity">Number of products</param>
        ///// <returns></returns>
        [HttpPost]
        public IActionResult Add(string id, string quantity)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product manager add.User: " + user);
            uint count = Convert.ToUInt32(quantity);
            var product = _context.ProductManagers.FirstOrDefault(pm => pm.ProductId == id);
            if (product != null)
            {
                var basket = _context.Baskets.Include(p => p.Product).FirstOrDefault(b => b.UserId == user.Id && b.ProductId == id);
                if (basket == null)
                {
                    basket = new Basket
                    {
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        AddDate = DateTime.Now,
                        Count = count,
                        ProductId = id,
                    };
                    _context.Baskets.Add(basket);
                    _context.SaveChanges();
                    return Json(true);
                }
                else
                {
                    basket.Count += count;
                    _context.Baskets.Update(basket);
                    _context.SaveChanges();
                    return Json(true);
                }
            }
            return Json(false);
        }


        
        /// <summary>
        /// Get max price in product manager
        /// </summary>
        /// <returns></returns>
        [Route("Products/MaxPrice")]
        public JsonResult GetMaxPrice()
        {
            decimal price = _context.ProductManagers.Max(p => p.SalePrice);
            return Json(price);
        }


        /// <summary>
        /// Get warehouse list
        /// </summary>
        /// <param name="testPMId">ProductManager Id</param>
        /// <returns></returns>
        public IActionResult WHList(string testPMId)
        {
            ViewBag.PMId = testPMId;
            return View("WHList",_context.Warehouses.ToList());
        }
        public IActionResult Move(string id, string IdOfPM)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var pm = _context.ProductManagers.FirstOrDefault(p => p.Id == IdOfPM);
            var wh = _context.Warehouses.FirstOrDefault(w => w.Id == id);
            pm.WareHouseId = wh.Id;
            _context.ProductManagers.Update(pm);
            _context.SaveChanges();
            id = pm.ProductId;
            _log.LogInformation("Product manager move to another warehouse.User: "+user);
            return RedirectToAction("Show", "ProductManager", new { id });
        }



        ///// <summary>
        ///// Write out ProductManager
        ///// </summary>
        ///// <param name="id">ProductManager Id</param>
        ///// <param name="quantity">Number of products</param>
        ///// <param name="price">Product price</param>
        ///// <returns></returns>
        [HttpPost]
        public IActionResult WriteOut(string id, string quantity, string price)
        {
            var productManager = _context.ProductManagers.Find(id);
            uint writeOutCount;
            decimal writeOutPrice;
            bool IsCount = UInt32.TryParse(quantity,out writeOutCount);
            bool IsNumber = Decimal.TryParse(price, out writeOutPrice);
            if (!IsCount || !IsNumber || productManager == null) return Json(false);
            _context.WriteOuts.Add(new WriteOut {
                Count = writeOutCount,
                Price = writeOutPrice,
                Date = DateTime.Now,
                ProductId = productManager.ProductId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                WarehouseId=productManager.WareHouseId,
            });
            productManager.CurrentCount -= writeOutCount;
            _context.Update(productManager);
            _context.SaveChanges();
            return Json(true);
        }
        [NonAction]
        bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                if (Request.Query.Count != 0)
                {
                    if (!(byte.TryParse(Request.Query["PageSize"], out byte size) && size > 0 && size < 101)) return false;
                    if (Request.Query.Keys.Contains("Page"))
                        if (!(uint.TryParse(Request.Query["Page"], out uint page) && page > 0)) return false;
                }
            }
            return true;
        }

    }
}
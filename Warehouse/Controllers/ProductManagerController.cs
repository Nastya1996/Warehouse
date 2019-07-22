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
                .Where(pm => pm.WareHouseId == user.WarehouseId && pm.Product.IsActive)
                .Include(p => p.Product.ProductType)
                .Include(p => p.Product.Unit).AsQueryable();
            if (viewModel.TypeId != null)
                if (_context.Types.Find(viewModel.TypeId) != null)
                    query = query.Where(pm => pm.Product.ProductTypeId == viewModel.TypeId);
                else return BadRequest();
            if (viewModel.ProductId != null)
                if (_context.Products.Find(viewModel.ProductId) != null)
                    query = query.Where(pm => pm.ProductId == viewModel.ProductId);
                else return BadRequest();
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
        public IActionResult Show(string id, decimal from, decimal before, int page=1, int pageSize=10)
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
            bool correct = UInt32.TryParse(quantity, out uint count);
            if (!correct || count == 0) return Json(false);
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
        public IActionResult WHList(string productId)
        {
            ViewBag.PMId = productId;
            var whId = _context.ProductManagers.FirstOrDefault(p => p.Id == productId).WareHouseId;
            return View("WHList",_context.Warehouses.Where(w=>w.Id != whId).ToList());
        }



        /// <summary>
        /// The Move method will mix the product from one warehouse to another
        /// </summary>
        /// <param name="id">The id parameter is the identifier of the warehouse where the product should be moved</param>
        /// <param name="IdOfPM">The IdOfPM parameter is the identifier of the ProductManager to be moved</param>
        /// <param name="count">the count parameter is the number of the moved product</param>
        /// <returns></returns>
        public IActionResult Move(string id, string IdOfPM, string count)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var productManager = _context.ProductManagers.Include(pm=>pm.Product).FirstOrDefault(p => p.Id == IdOfPM);
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.Id == id);
            bool isCount = uint.TryParse(count, out uint CountOfProduct);
            if (productManager == null || warehouse == null || !isCount || CountOfProduct==0)
                return BadRequest();
            if (productManager.CurrentCount == CountOfProduct)
            {
                productManager.Count = CountOfProduct;
                productManager.WareHouseId = id;
            }
            else
            {
                productManager.Count -=CountOfProduct;
                productManager.CurrentCount -= CountOfProduct;
                _context.Update(productManager);
                _context.ProductManagers.Add(new ProductManager {
                    Count = CountOfProduct,
                    CurrentCount = CountOfProduct,
                    ReceiptDate = productManager.ReceiptDate,
                    Date = DateTime.Now,
                    UserId = user.Id,//sa mehet eshili kariq one, ete min user texapoxuma apranqy, enmin pahestum huva yndunum?
                    WareHouseId=id,
                    ReceiptPrice=productManager.ReceiptPrice,
                    SalePrice=productManager.SalePrice,
                    ProductId=productManager.ProductId
                });
            }
            _context.ProductMoves.Add(new ProductMove
            {
                BeforeId = user.WarehouseId,
                AfterId = id,
                Date = DateTime.Now,
                UserId = user.Id,
                Count = CountOfProduct,
                ProductId=productManager.ProductId,
                TypeId=productManager.Product.ProductTypeId
            });
            _context.SaveChanges();
            id = productManager.ProductId;
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
                var keys = Request.Query.Keys;
                var request = Request.Query;
                if(keys.Contains("PageSize"))
                    if (!(byte.TryParse(request["PageSize"], out byte size) && size > 0 && size < 101)) return false;
                if (keys.Contains("Page"))
                    if (!(uint.TryParse(Request.Query["Page"], out uint page) && page > 0)) return false;
            }
            return true;
        }

    }
}
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

        public IActionResult Index(string type, string name, int page=1, int pageSize=10)
        {
            type = type == null ? "" : type.Trim();
            name = name == null ? "" : name.Trim();
            ViewData["CurrentType"] = type;
            ViewData["CurrentName"] = name;
            ViewData["CurrentSize"] = pageSize;
            var user=_context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var productManagersByGroup = _context.ProductManagers
                .Where(pm => pm.WareHouseId == user.WarehouseId && pm.Product.IsActive!=false)
                .Include(p => p.Product)
                .Include(p => p.Product.ProductType)
                .Include(p=>p.Product.Unit)
                .GroupBy(pm => pm.Product)
                .Where(s => s.Key.ProductType.Name.Contains(type, StringComparison.InvariantCultureIgnoreCase) &&
                s.Key.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                .Select(item => new
                {
                    Product = item.Key,
                    Count = item.Sum(p => p.Count),
                    CurrentCount = item.Sum(p => p.CurrentCount)

                })
                .Select(c => c.ToExpando());
            PagedList<ExpandoObject> model = new PagedList<ExpandoObject>(productManagersByGroup, page, pageSize);
            _log.LogInformation("Product manager index.User: "+user);
            return View(model);
        }
        //Create
        [Authorize(Roles = "Storekeeper")]
        [HttpGet]
        public IActionResult Create()
        {
            SelectInitial();
            return View();
        }
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
                productManager.AddDate = DateTime.Now;
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
        void SelectInitial()
        {
            ViewBag.WareHouses = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name");
        }

        //Show Product
        public IActionResult Show(string id, decimal from, decimal before, int page=1, int pageSize=1)
        {
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


        //Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var productManager = _context.ProductManagers.Find(id.ToString());
            if(productManager!=null)
                return View(productManager);
            return RedirectToAction("Index");
        }

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

        //Add to basket
        [Route("ProductManager/Add/{id}/{quantity}")]
        [HttpGet]
        public IActionResult Add(string id, string quantity)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Product manager add.User: "+user);
            uint count = Convert.ToUInt32(quantity);
            var product = _context.ProductManagers.FirstOrDefault(pm => pm.ProductId == id);
            if (product != null)
            {
                var basket = _context.Baskets.Include(p=>p.Product).FirstOrDefault(b => b.UserId == user.Id && b.ProductId == id);
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


        //Get Products
        [HttpPost]
        [Route("Products/Get")]
        public JsonResult GetProduct([FromBody]string selected)
        {
            return Json(_context.Products.Where(p => p.ProductTypeId == selected && p.IsActive != false).ToList());
        }

        [Route("Products/MaxPrice")]
        public JsonResult GetMaxPrice()
        {
            decimal price = _context.ProductManagers.Max(p => p.SalePrice);
            return Json(price);
        }
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
    }
}
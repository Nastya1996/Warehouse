using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.HtmlHelper;
using Warehouse.Models;
namespace Warehouse.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductManagerController(ApplicationDbContext context) => _context = context;

        public IActionResult Index()
        {
            var user=_context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var productManagersByGroup = _context.ProductManagers.Where(pm => pm.WareHouseId == user.WarehouseId)
                .Include(p => p.Product)
                .Include(p => p.Product.ProductType)
                .Include(p=>p.Product.Unit)
                .GroupBy(pm => pm.Product)
                .Select(item => new
                {
                    Product = item.Key,
                    Count = item.Sum(p => p.Count),
                    CurrentCount = item.Sum(p => p.CurrentCount)

                })
                .Select(c => c.ToExpando());
            return View(productManagersByGroup.ToList());
        }
        
        //Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.WareHouses = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users, nameof(AppUser.Id), nameof(AppUser.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductManager productManager)
        {
            var product = _context.Products.Include(u=>u.Unit).FirstOrDefault(p=>p.Id==productManager.ProductId);
            productManager.AddDate = DateTime.Now;
            productManager.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            productManager.CurrentCount = productManager.Count;
            productManager.Product = product;
            _context.Add(productManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Show Product
        public IActionResult Show(string id)
        {
            var products = _context.ProductManagers.Where(p => p.ProductId == id)
                .Include(p=>p.Product).Include(u=>u.Product.Unit).ToList();
            return View(products);
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
            return RedirectToAction("Show", new Dictionary<string, string> { { "id", prodManager.ProductId} });
        }
        
        

        [Route("ProductManager/Add/{id}/{quantity}")]
        [HttpGet]
        public IActionResult Add(string id, string quantity)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            uint count = Convert.ToUInt32(quantity);
            var product = _context.ProductManagers.FirstOrDefault(pm => pm.ProductId == id);
            if (product != null)
            {
                var basket = _context.Baskets.Include(p=>p.ProductBaskets).FirstOrDefault(b => b.UserId == user.Id);
                if (basket == null)
                {
                    basket = new Basket
                    {
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    };
                    _context.Baskets.Add(basket);
                    _context.SaveChanges();
                }
                var productInBasket = basket.ProductBaskets.FirstOrDefault(pb => pb.ProductId == id);
                if (productInBasket == null) basket.ProductBaskets.Add(new ProductBasket
                {
                    Count = count,
                    ProductId = id,
                    AddDate = DateTime.Now
                });
                else productInBasket.Count += count;
                _context.Baskets.Update(basket);
                _context.SaveChanges();

            }
            return new JsonResult("");
        }


        [HttpPost]
        [Route("Products/Get")]
        //Get Products
        public JsonResult GetProduct([FromBody]string selected)
        {
            return Json(_context.Products.Where(p => p.ProductTypeId == selected).ToList());
        }
    }
}
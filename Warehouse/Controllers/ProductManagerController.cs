using System;
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
                .GroupBy(pm => pm.Product)
                .Select(item => new
                {
                    Product = item.Key,
                    Count = item.Sum(p => p.Count),
                    CurrentCount = item.Sum(p => p.CurrentCount)

                })
                .Select(c => c.ToExpando());
            //SortState sortOrder = SortState.ProductNameAsc
            //IQueryable<ProductManager> products = _context.ProductManagers.Include(x => x.Product).Include(p => p.Product.ProductType).Include(x => x.WareHouse).Include(x => x.User);
            //ViewBag.ProductNameSort = sortOrder == SortState.ProductNameAsc ? SortState.ProductNameDesc : SortState.ProductNameAsc;

            //switch (sortOrder)
            //{
            //    case SortState.ProductNameDesc:
            //        products = products.OrderByDescending(s => s.Product.Name);
            //        break;
            //}

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
            productManager.AddDate = DateTime.Now;
            productManager.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            productManager.CurrentCount = productManager.Count;
            _context.Add(productManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Show Product
        public IActionResult Show(string id)
        {
            var products = _context.ProductManagers.Where(p => p.ProductId == id)
                .Include(p=>p.Product).ToList();
            return View(products);
        }


        //Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var productManager = _context.ProductManagers
                .Include(p => p.Product)
                .FirstOrDefault(x => x.Id == id);
            //ViewBag.WareHouses = new SelectList(_context.Warehouses, "Id", "Number");
            //ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            //ViewBag.ProductTypes = new SelectList(_context.Types, "Id", "Name", productManager.Product.ProductTypeId);
            //ViewBag.Users = new SelectList(_context.Users, nameof(AppUser.Id), nameof(AppUser.Name));
            //return View();
            //return View(productManager);
            return RedirectToAction("Index", "ProductInfo",new {id=id });
        }

        [HttpPost]
        public IActionResult Edit(ProductManager productManager)
        {
            var prodManager = _context.ProductManagers
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == productManager.Id);
            //productManager.AddedDate = prodManager.AddedDate;
            productManager.CurrentCount = productManager.Count;
            _context.Update(productManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Delete
        //[HttpGet]
        //public IActionResult Delete(string id)
        //{
        //    var productManager = _context.ProductManagers.Find(id);
        //    if (productManager == null) return NotFound();
        //    return View(productManager);
        //}

        //[HttpGet]
        //public IActionResult DeleteProductManager(string id)
        //{
        //    var productManager = _context.ProductManagers.Find(id);
        //    if (productManager == null) return NotFound();
        //    _context.Remove(productManager);
        //    _context.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            //var productManager = _context.ProductManagers.Include(pt => pt.Product).
            //    Include(w => w.WareHouse).Include(u => u.User).FirstOrDefault(x => x.Id == id);
            //if (productManager == null) return NotFound();
            //return View(productManager);
            return View();
        }



        [Route("ProductManager/Add/{id}/{quantity}")]
        [HttpGet]
        public IActionResult Add(string id, string quantity)
        {
            Basket basket;
            uint count = Convert.ToUInt32(quantity);
            var product = _context.ProductManagers.Find(id);
            if (product != null)
            {
                var productInBasket = _context.Baskets.FirstOrDefault(pm => pm.ProductManagerId == id);
                if (productInBasket==null)
                {
                    basket = new Basket
                    {
                        Quantity = count,
                        ProductManagerId = product.Id,
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    };
                    _context.Baskets.Add(basket);
                    _context.SaveChanges();
                }
                else
                {
                    if (productInBasket.Quantity + count <= product.Count) 
                         productInBasket.Quantity += count;
                    else productInBasket.Quantity = product.Count;
                    _context.Update(productInBasket);
                    _context.SaveChanges();
                }
            }
            return new JsonResult("");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Orders.Where(o=>!o.IsSelled));
        }
        [HttpGet]
        public IActionResult Create()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var basket = _context.Baskets.Include(pb => pb.Product).Where(b => b.UserId == user.Id);
            var baskets = new List<Basket>();

            var productOrderList = new List<ProductOrder>();
            foreach (var item in basket)
            {
                var count = item.Count;

                var productManager = _context.ProductManagers.Where(p => p.WareHouseId == user.WarehouseId && p.ProductId == item.ProductId).OrderBy(pm => pm.AddDate).ToList();
                if (productManager.Sum(p => p.CurrentCount) < count)
                {
                    baskets.Add(item);
                }
                else
                {

                    for (int i = 0; i < productManager.Count; ++i)
                    {
                        if (productManager[i].CurrentCount >= count)
                        {
                            productOrderList.Add(new ProductOrder()
                            {
                                Count = count,
                                ProductId = item.ProductId,
                                Product = item.Product,
                                Price = productManager[i].SalePrice,
                                FinallyPrice = count * productManager[i].SalePrice,
                                ProductManagerId = productManager[i].Id
                            });
                            count = 0;
                            break;
                        }
                        else
                        {
                            productOrderList.Add(new ProductOrder()
                            {
                                Count = productManager[i].CurrentCount,
                                ProductId = item.ProductId,
                                Product = item.Product,
                                Price = productManager[i].SalePrice,
                                FinallyPrice = productManager[i].CurrentCount * productManager[i].SalePrice,
                                ProductManagerId = productManager[i].Id
                            });
                            count -= productManager[i].CurrentCount;
                        }
                    }
                }
            }
            var order = new Order()
            {
                Date = DateTime.Now,
                FinallPrice = productOrderList.Sum(p => p.FinallyPrice),
                Price = productOrderList.Sum(p => p.FinallyPrice),
                ProductOrders = productOrderList,
                UserId = user.Id
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            ViewBag.Baskets = baskets;
            return View(order);

        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            var productManagers = new List<ProductManager>();
            var orderDb = _context.Orders.FirstOrDefault(o => o.Id == order.Id && !o.IsSelled);
            if (orderDb == null)
                return NotFound();
            foreach (var item in orderDb.ProductOrders)
            {

                var productManager = _context.ProductManagers.FirstOrDefault(p => p.Id == item.ProductManagerId);
                if (productManager != null)
                {
                    if (productManager.CurrentCount >= item.Count)
                    {
                        productManager.CurrentCount -= item.Count;
                        productManagers.Add(productManager);
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    return NotFound();
                }
                var sale = order.ProductOrders.FirstOrDefault(p => p.Id == item.Id).Sale;
                item.FinallyPrice = item.Price * sale;
            }

            orderDb.Price = order.ProductOrders.Sum(p => p.FinallyPrice);
            orderDb.FinallPrice = orderDb.Price * order.Sale;

            _context.ProductManagers.UpdateRange(productManagers);
            _context.Orders.Update(orderDb);
            _context.SaveChanges();

            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var basket = _context.Baskets.Include(pb => pb.Product).FirstOrDefault(b => b.UserId == user.Id);
            _context.Baskets.Remove(basket);
            _context.SaveChanges();


            return View("ProductManager/Index");
        }
    }
}
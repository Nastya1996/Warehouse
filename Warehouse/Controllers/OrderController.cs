using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Worker")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(_context.Orders.Where(o => o.UserId == user.Id).ToList());
        }
        [HttpGet]
        public IActionResult Back(string id)
        {
            var productOrders = _context.ProductOrders
                .Include(p=>p.Product)
                .Include(pm=>pm.ProductManager)
                .Include(pm=>pm.ProductManager.User)
                .Include(pt=>pt.Product.ProductType)
                .Include(u=>u.Product.Unit)
                .Where(po=>po.OrderId == id);
            return View("Show", productOrders);
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

                var productManager = _context.ProductManagers.Where(p => p.WareHouseId == user.WarehouseId && p.ProductId == item.ProductId && p.CurrentCount!=0).OrderBy(pm => pm.AddDate).ToList();
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
                    _context.Baskets.Remove(item);
                }
            }
            var order = new Order()
            {
                Date = DateTime.Now,
                FinallPrice = productOrderList.Sum(p => p.FinallyPrice),
                Price = productOrderList.Sum(p => p.FinallyPrice),
                ProductOrders = productOrderList,
                UserId = user.Id,
                OrderType=OrderType.InProgress
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            ViewBag.Baskets = baskets;
            
            //_context.Baskets.RemoveRange(basket);
            _context.SaveChanges();
            return View(order);

        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            var productManagers = new List<ProductManager>();
            var orderDb = _context.Orders.Include(p=>p.ProductOrders).FirstOrDefault(o => o.Id == order.Id && o.OrderType==OrderType.InProgress);
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
                        Con(order.Id);
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    return NotFound();
                }
                var sale = order.ProductOrders.FirstOrDefault(p => p.Id == item.Id).Sale;
                item.FinallyPrice = item.Price * (100-sale)/100;
            }

            orderDb.Price = orderDb.ProductOrders.Sum(p => p.FinallyPrice);
            orderDb.FinallPrice = orderDb.Price * (100 - order.Sale)/100;
            orderDb.OrderType = OrderType.Saled;

            _context.ProductManagers.UpdateRange(productManagers);
            _context.Orders.Update(orderDb);
            _context.SaveChanges();


            return RedirectToAction("Index","ProductManager");
        }
        private void Con(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = _context.Orders.Include(p => p.ProductOrders).FirstOrDefault(o => o.Id == id && o.UserId == user.Id);
            var baskets = new List<Basket>();


            var productOrdersAndManager = order.ProductOrders.GroupBy(p => p.ProductId).ToDictionary(key => key.Key, value => value.ToList());
            foreach (var key in productOrdersAndManager.Keys)
            {
                baskets.Add(new Basket
                {
                    AddDate = DateTime.Now,
                    Count = (uint)productOrdersAndManager[key].Sum(count => count.Count),
                    ProductId = key,
                    UserId = order.UserId
                });
            }

            _context.Orders.Remove(order);
            _context.Baskets.AddRange(baskets);
            _context.SaveChanges();
        }
        public IActionResult Continue(string id)
        {
            Con(id);
            return RedirectToAction("Index", "ProductManager");
        }
        public IActionResult Delete(string id)
        {
            _context.Orders.Remove(_context.Orders.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
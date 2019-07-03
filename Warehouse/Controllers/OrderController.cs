using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Data;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Worker")]
    public class OrderController : Controller
    {
        readonly ILogger<OrderController> _log;
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context, ILogger<OrderController> log)
        {
            _log = log;
            _context = context;
        }
        public IActionResult Index()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Order index."+user);
            return View(_context.Orders.Where(o => o.UserId == user.Id).OrderByDescending(o=>o.Date).ToList());
        }
        [HttpGet]
        public IActionResult Back(string id)
        {
            if (_context.Orders.Find(id) == null)
                return BadRequest();
            var productOrders = _context.ProductOrders
                .Include(p=>p.Product)
                .Include(pm=>pm.ProductManager)
                .Include(pm=>pm.ProductManager.User)
                .Include(pt=>pt.Product.ProductType)
                .Include(u=>u.Product.Unit)
                .Where(po=>po.OrderId == id);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Back product."+user);
            return View("Show", productOrders);
        }
        [HttpPost]
        public JsonResult FinallyBack(string id, string count)
        {
            UInt32 countCast;
            if (!UInt32.TryParse(count, out countCast))
                return new JsonResult(false);
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var po = _context.ProductOrders
                .Include(o => o.Order)
                .FirstOrDefault(p => p.Id == id);
            if (po == null)
                return new JsonResult(false);
            var pm = _context.ProductManagers.FirstOrDefault(p => p.Id == po.ProductManagerId);
            if (po.Count - po.ReturnedCount < countCast)
                return new JsonResult(false);
            if (pm.WareHouseId == po.Order.WareHouseId)
            {
                pm.CurrentCount += countCast;
                _context.ProductManagers.Update(pm);
            }
            else
            {
                var newPM = new ProductManager
                {
                    Date = pm.Date,
                    Count = countCast,
                    CurrentCount = countCast,
                    ProductId = pm.ProductId,
                    ReceiptDate = pm.ReceiptDate,
                    ReceiptPrice = pm.ReceiptPrice,
                    SalePrice = pm.SalePrice,
                    UserId = pm.UserId,
                     WareHouseId = po.Order.WareHouseId
                };
                _context.ProductManagers.Add(newPM);

            }
            //todo:: change orders data
            po.Order.Price -= countCast * po.FinallyPrice;
            po.Order.FinallPrice-= countCast * po.FinallyPrice * (100 - po.Sale)/100;
            po.ReturnedCount += countCast;
            _context.ProductOrders.Update(po);
            _context.SaveChanges();
            _log.LogInformation("Back product."+user);
            return new JsonResult(true);
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

                var productManager = _context.ProductManagers.Where(p => p.WareHouseId == user.WarehouseId && p.ProductId == item.ProductId && p.CurrentCount!=0).OrderBy(pm => pm.Date).ToList();
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
                OrderType=OrderType.InProgress,
                WareHouseId=user.WarehouseId
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            ViewBag.Baskets = baskets;
            ViewBag.Custemers = new SelectList(_context.Customers.ToList(), "Id", "FullName");
            
            //_context.Baskets.RemoveRange(basket);
            _context.SaveChanges();
            _log.LogInformation("Created order."+user);
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
                item.Price = productManager.SalePrice;
                var productOrder = order.ProductOrders.FirstOrDefault(po => po.Id == item.Id);

                item.FinallyPrice = productManager.SalePrice * (100 - (productOrder == null ? 0 : productOrder.Sale)) / 100;
            }

            //orderDb.Price = orderDb.ProductOrders.Sum(p => p.FinallyPrice * p.Count);
            var price = orderDb.ProductOrders.Sum(p => p.FinallyPrice * p.Count);
            orderDb.FinallPrice = price * (100 - order.Sale)/100;
            orderDb.OrderType = OrderType.Saled;
            orderDb.CustomerId = order.CustomerId;
            orderDb.Sale = order.Sale;

            _context.ProductManagers.UpdateRange(productManagers);
            _context.Orders.Update(orderDb);
            _context.SaveChanges();
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Check out."+user);
            return RedirectToAction("Index","Order");
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
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Continue order."+user);
            return RedirectToAction("Index", "Basket");
        }


        [HttpPost]
        public JsonResult Delete(string id)
        {
            var order = _context.Orders.Find(id);
            if (order==null) return Json(false);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Deleted order."+user);
            return Json(true);
        }
    }
}
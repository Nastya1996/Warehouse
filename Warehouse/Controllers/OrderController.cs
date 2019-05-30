using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(IEnumerable<Basket> model)
        {
            var obj = _context.Orders;
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var basket = _context.Baskets.Include(pb => pb.ProductBaskets).FirstOrDefault(b=>b.UserId == user.Id);

            var productOrderList = new List<ProductOrder>();
            foreach (var item in basket.ProductBaskets)
            {

                var productManager = _context.ProductManagers.Where(p => p.WareHouseId == user.WarehouseId && p.ProductId == item.ProductId).OrderBy(pm=>pm.AddDate).ToList();

                for (int i=0; i<productManager.Count;++i)
                {
                    if (productManager[i].CurrentCount >= item.Count)
                    {
                        productOrderList.Add(new ProductOrder()
                        {
                            Count = item.Count,
                            ProductId = item.ProductId,
                            Price = productManager[i].SalePrice,
                            FinallyPrice = item.Count * productManager[i].SalePrice
                        });
                        break;
                    }
                    else
                    {
                        productOrderList.Add(new ProductOrder()
                        {
                            Count = productManager[i].CurrentCount,
                            ProductId = item.ProductId,
                            Price = productManager[i].SalePrice,
                            FinallyPrice = productManager[i].CurrentCount * productManager[i].SalePrice
                        });
                        item.Count -= productManager[i].CurrentCount;
                    }

                }
            }
            Order order = new Order() {
                Date = DateTime.Now,
                FinallPrice = productOrderList.Sum(p=>p.FinallyPrice),
                Price = productOrderList.Sum(p => p.FinallyPrice),
                ProductOrders = productOrderList,
                UserId = user.Id  
            };
            return View(order);
        }
    }
}
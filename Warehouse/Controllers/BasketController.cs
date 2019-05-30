using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Warehouse.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public string MemberShip { get; private set; }

        public BasketController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Baskets.Include(x=>x.ProductBaskets));//..ProductManager.Product.Unit
        }
        public IActionResult Delete(string id)
        {
            return View(_context.Baskets.Find(id));
        }
        public IActionResult DeleteYes(string id)
        {
            _context.Baskets.Remove(_context.Baskets.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
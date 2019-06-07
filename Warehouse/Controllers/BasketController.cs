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
using Microsoft.AspNetCore.Authorization;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Worker")]
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
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //todo
            var baskets = _context.Baskets.Include(p => p.Product).Where(p=>p.UserId == user.Id);

            return View("_Index", baskets);
            //return View();
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
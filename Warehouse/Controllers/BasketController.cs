﻿using System;
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
using Microsoft.Extensions.Logging;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Worker")]
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly ILogger<BasketController> _log;

        public string MemberShip { get; private set; }

        public BasketController(ApplicationDbContext context, ILogger<BasketController> log)
        {
            _log = log;
            _context = context;
        }
        public IActionResult ChangeCount(string id, string count)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var basket = _context.Baskets.Find(id);
            basket.Count = Convert.ToUInt32(count);
            _context.Baskets.Update(basket);
            _context.SaveChanges();
            var baskets = _context.Baskets.Include(p => p.Product).Where(p => p.UserId == user.Id);
            return View("_Index", baskets);
        }
        public IActionResult Index()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //todo
            var baskets = _context.Baskets.Include(p => p.Product).Where(p=>p.UserId == user.Id).ToList();
            _log.LogInformation("Basket index. User: "+user);
            return View("_Index", baskets);
        }
        public IActionResult IndexForHover()
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //todo
            var baskets = _context.Baskets.Include(p => p.Product).Where(p => p.UserId == user.Id);
            _log.LogInformation("Basket index.User: "+user);
            return View("_IndexForHover", baskets);
        }
        [HttpPost]
        public IActionResult DeleteYes(string id)
        {
            var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var baskets2 = _context.Baskets.Include(p => p.Product).Where(p => p.UserId == user.Id);
            var obj = _context.Baskets.FirstOrDefault(b=>b.Id == id);
            if(obj!=null)
                _context.Baskets.Remove(obj);
            _context.SaveChanges();
            
            _log.LogInformation("Deleted basket item."+user);
            var baskets = _context.Baskets.Include(p => p.Product).Where(p => p.UserId == user.Id);
            _log.LogInformation("Basket index.User: " + user);
            return View("_IndexForHover", baskets);
        }
    }
}
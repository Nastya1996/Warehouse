using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ShowUsers()
        {
            var c = _context.AppUsers.Include(user => user.Warehouse).ToList();
            return View(c);
        }
        public IActionResult CreateAdmin()
        {
            return View();
        }


        //Users disable and enable
        [HttpPost]
        [Route("User/Disable/")]
        public JsonResult Disable([FromBody]string userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
                return Json(false);
            else
            {
                user.LockoutEnd = DateTime.MaxValue;
                _context.Update(user);
                _context.SaveChanges();
                return Json(true);
            }
        }

        [HttpPost]
        [Route("User/Enable/")]
        public JsonResult Enable([FromBody]string userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
                return Json(false);
            else if (user.LockoutEnd == null)
                return Json(false);
            user.LockoutEnd = null;
            _context.Update(user);
            _context.SaveChanges();
            return Json(true);
        }
        public IActionResult WHListForAdmin(string userId)
        {
            ViewBag.UserID = userId;
            return View("WHListForAdmin", _context.Warehouses.ToList());
        }
        public IActionResult Move(string id, string IdOfUser)
        {
            var users = _context.AppUsers.FirstOrDefault(u => u.Id == IdOfUser);
            var wh = _context.Warehouses.FirstOrDefault(w => w.Id == id);
            users.WarehouseId = wh.Id;
            _context.AppUsers.Update(users);
            _context.SaveChanges();
            return RedirectToAction("ShowUsers", "Admin");
        }
    }
}
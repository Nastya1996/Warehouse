using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(_context.Users.Where(u=>u.Id!=userId).ToList());
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
    }
}
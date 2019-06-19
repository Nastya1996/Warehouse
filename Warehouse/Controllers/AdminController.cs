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
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult ShowUsers(string name="", string role = "", string number= "", int page = 1, int pageSize = 10)
        {

            name = name == null ? "" : name.Trim();
            //role = role == null ? "" : role.Trim();
            number = number == null ? "" : number.Trim();

            // var users = //_context.AppUsers
            //                            .Include(w => w.Warehouse)
            //                            .Where(us => us.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)
            //                                && us.Warehouse != null && us.Warehouse.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase)).ToList();
            IEnumerable<string> userIds = null;

            if (role != null)
            {
                var roleDb = _roleManager.Roles.FirstOrDefault(rl => rl.Name.Contains(role, StringComparison.InvariantCultureIgnoreCase));
                if (roleDb != null)
                    userIds = _context.UserRoles.Where(r => r.RoleId == roleDb.Id)
                        .Select(i => i.UserId)
                        .ToList();
            }

            IEnumerable<AppUser> users;

            if(userIds!=null)
            {
                users = _context.AppUsers
                .Include(w => w.Warehouse)
                .Where(u => u.UserName.Contains(name) && userIds.Contains(u.Id) && u.Warehouse.Number.Contains(number));
            }
            else
                users =_context.AppUsers
                .Include(w => w.Warehouse)
                .Where(u => u.UserName.Contains(name));



            ViewData["CurrentName"] = name;
            ViewData["CurrentRole"] = role;
            ViewData["CurrentNumber"] = number;
            ViewData["CurrentSize"] = pageSize;

           // var users = _context.AppUsers.Include(user => user.Warehouse).ToList();
            PagedList<AppUser> model = new PagedList<AppUser>(users, page, pageSize);

            return View(model);
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
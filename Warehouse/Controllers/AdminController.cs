﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Infrastructure;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _log;
        public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> log)
        {
            _log = log;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //public IActionResult ShowUsers(string name = "", string role = "", string number = "", int page = 1, int pageSize = 10)
        //{

        //    name = name == null ? "" : name.Trim();
        //    number = number == null ? "" : number.Trim();
        //    IEnumerable<string> userIds = null;

        //    if (role != null)
        //    {
        //        var roleDb = _roleManager.Roles.FirstOrDefault(rl => rl.Name.Contains(role, StringComparison.InvariantCultureIgnoreCase));
        //        if (roleDb != null)
        //            userIds = _context.UserRoles.Where(r => r.RoleId == roleDb.Id)
        //                .Select(i => i.UserId)
        //                .ToList();
        //    }

        //    IEnumerable<AppUser> users;
        //    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    if (userIds != null)
        //    {
        //        users = _context.AppUsers
        //        .Include(w => w.Warehouse)
        //        .Where(u => u.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase) && userIds.Contains(u.Id) && u.Warehouse.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase) && u.Id!=currentUserId);
        //    }
        //    else { 
        //        users = _context.AppUsers
        //        .Include(w => w.Warehouse)
        //        .Where(u => u.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase) && u.Warehouse.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase) && u.Id!=currentUserId);
        //    }


        //    ViewData["CurrentName"] = name;
        //    ViewData["CurrentRole"] = role;
        //    ViewData["CurrentNumber"] = number;
        //    ViewData["CurrentSize"] = pageSize;

        //   // var users = _context.AppUsers.Include(user => user.Warehouse).ToList();
        //    PagedList<AppUser> model = new PagedList<AppUser>(users, page, pageSize);
        //    var userSignIn = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    _log.LogInformation("Show users.User: "+ userSignIn);
        //    return View(model);
        //}

        public IActionResult ShowUsers(UsersViewModel viewModel)
        {
            
            foreach (var item in _context.AppUsers)
            {
                var li = new List<WareHouse>();
                foreach (var wh in _context.AppUserWareHouses.Where(u=>u.AppUserId == item.Id))
                {
                    li.Add(_context.Warehouses.Find(wh.WareHouseId));
                }
                item.WareHouses = li;
            }
            
            if (!FilterValid()) return BadRequest();
            var userSignIn = _context.AppUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var query = _context.AppUsers.Where(u => u.Id != userSignIn.Id).ToList();
            if (!string.IsNullOrEmpty(viewModel.Name))
            {
                viewModel.Name = viewModel.Name.Trim();
                query = query.Where(u => u.Name.Contains(viewModel.Name,StringComparison.InvariantCultureIgnoreCase)
                                      || u.SurName.Contains(viewModel.Name,StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            
            if (viewModel?.WareHouses?.Count != 0 && viewModel?.WareHouses!=null)
            {
                List<AppUser> us = new List<AppUser>();
                foreach (var item in viewModel.WareHouses)
                {
                    if (item != null)
                    {
                        var obj = _context.AppUsers.Where(u => u.WareHouses.Contains(_context.Warehouses.FirstOrDefault(w => w.Id == item)));
                        if (obj.Count() > 0)
                            foreach (var objItem in obj)
                            {
                                us.Add(objItem);
                            }
                    }
                }
                query = us;
                if (us.Count == 0)
                    query = _context.AppUsers.Where(u => u.Id != userSignIn.Id).ToList();
            }
            
            
            var usersId = _context.UserRoles.Where(ur => ur.RoleId == viewModel.RoleId).Select(u=>u.UserId);
            if (viewModel.RoleId != null)
                if (_context.Roles.Find(viewModel.RoleId) != null)
                    query = query.Where(u => usersId.Contains(u.Id)).ToList();
                else return BadRequest();
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            ViewBag.Numbers = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.paged = new PagedList<AppUser>(query.AsQueryable(), viewModel.Page, viewModel.PageSize);
            _log.LogInformation("Show users.User: " + userSignIn);
            return View(viewModel);
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
            if (user == null || user.LockoutEnd!=null)
                return Json(false);
            else
            {
                user.LockoutEnd = DateTime.MaxValue;
                _context.Update(user);
                _context.SaveChanges();
                var userSignIn = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Disabled user.User: "+ userSignIn);
                return Json(true);
            }
        }

        [HttpPost]
        [Route("User/Enable/")]
        public JsonResult Enable([FromBody]string userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null || user.LockoutEnd == null)
                return Json(false);
            user.LockoutEnd = null;
            _context.Update(user);
            _context.SaveChanges();
            var userSignIn = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _log.LogInformation("Enabled user.User: "+ userSignIn);
            return Json(true);
        }
        public IActionResult WHListForAdmin(string userId)
        {
            ViewBag.UserID = userId;
            //var whIdOfUser = _context.Users.FirstOrDefault(u=>u.Id == userId).WarehouseId;
            //var wh = _context.Warehouses.Where(w=>w.Id != whIdOfUser).ToList();
            return View("WHListForAdmin");
        }
        public IActionResult Move(string id, string IdOfUser)
        {
            if (id != null)
            {
                var users = _context.AppUsers.FirstOrDefault(u => u.Id == IdOfUser);
                var wh = _context.Warehouses.FirstOrDefault(w => w.Id == id);
                //users.WarehouseId = wh.Id;
                _context.AppUsers.Update(users);
                _context.SaveChanges();
                var user = _context.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _log.LogInformation("Changed user's warehouse.User: " + user);
            }
            return RedirectToAction("ShowUsers", "Admin");
        }
        [NonAction]
        bool FilterValid()
        {
            if (Request.Query.Count != 0)
            {
                var keys = Request.Query.Keys;
                var request = Request.Query;
                if (keys.Contains("PageSize"))
                    if (!(byte.TryParse(request["PageSize"], out byte size) && size > 0 && size < 101)) return false;
                if (keys.Contains("Page"))
                    if (!(uint.TryParse(request["Page"], out uint page) && page > 0)) return false;
            }
            return true;
        }
    }
}
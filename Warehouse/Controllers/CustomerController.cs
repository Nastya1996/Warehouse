using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "Worker")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var customers = _context.Customers.AsQueryable();
            ViewData["CurrentSize"] = pageSize;
            PagedList<Customer> model = new PagedList<Customer>(customers, page, pageSize);
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(string id)
        {
            var model = _context.Customers.Find(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(string id)
        {
            var model = _context.Customers.Find(id);
            return View(model);
        }

        public IActionResult DeleteCustomer(string id)
        {
            var obj = _context.Customers.Find(id);
            if (obj != null)
            {
                _context.Customers.Remove(obj);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Back()
        {
            return RedirectToAction("Index");
        }
        public IActionResult Details(string id) {
            var customer = _context.Customers.Find(id);
            return View(customer);
        }

        public JsonResult Register(Customer customer)
        {
            if (_context.Customers.FirstOrDefault(c => c.Phone == customer.Phone) != null)
                return Json(false);
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult Get()
        {
            return Json(_context.Customers.AsQueryable());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class CustomerController : Controller

    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var datas = _context.Customers.ToList();
            return View(datas);
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
    }
}
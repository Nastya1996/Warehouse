using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;
namespace Warehouse.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductManagerController(ApplicationDbContext context) => _context = context;

        //Index
        public IActionResult Index()
        {
            return View(_context.ProductManagers.Include(x=>x.WareHouse).ToList());
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.WareHouse = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.Product = new SelectList(_context.ProductCustomers, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductManager productManager)
        {
            ViewBag.Warehouses = new SelectList(_context.Warehouses,"Id","Number");
            ViewBag.Product = new SelectList(_context.ProductCustomers, "Id", "Name");
            _context.Add(productManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBag.WareHouse = new SelectList(_context.Warehouses, "Id", "Number");
            ViewBag.Product = new SelectList(_context.ProductCustomers, "Id", "Name");
            var pm = _context.ProductManagers.Include(x => x.WareHouse).FirstOrDefault(x => x.Id == id);
            return View(pm);//.Include(x=>x.UserId)
        }
        [HttpPost]
        public IActionResult Edit(ProductManager productManager)
        {
            _context.Update(productManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Delete
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var data = _context.ProductManagers.Find(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpGet]
        public IActionResult DeleteProductManager(string id)
        {
            var data = _context.ProductManagers.Find(id);
            if (data == null) return NotFound();
            _context.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //Details
        [HttpGet]
        public IActionResult Details(string id)
        {
            return View(_context.ProductManagers.Include(x => x.WareHouse).FirstOrDefault(x => x.Id == id));//.Include(x=>x.UserId))
        }
    }
}
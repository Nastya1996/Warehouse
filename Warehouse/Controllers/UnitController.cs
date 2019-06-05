﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Authorize(Roles = "storekeeper")]
    public class UnitController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UnitController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var unitDatas = _context.Units;
            return View(unitDatas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Unit unit)
        {
            if (ModelState.IsValid) {
                _context.Add(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(string id)
        {
            var obj = _context.Units.Find(id);
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Units.Update(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(string id)
        {
            return View(_context.Units.Find(id));
        }
        public IActionResult DeleteYes(string id)
        {
            _context.Units.Remove(_context.Units.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(string id)
        {
            var obj = _context.Units.Find(id);
            return View(obj);
        }
    }
}
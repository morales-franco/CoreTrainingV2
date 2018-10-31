﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private INullMailService _nullMailService;
        private DutchContext _context;

        public AppController(INullMailService nullMailService, DutchContext context)
        {
            _nullMailService = nullMailService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            //throw new InvalidOperationException("Bad things happened");
            return View();
        }

        [HttpPost("Contact")]
        public IActionResult Contact(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                //Send email
                _nullMailService.SendMessage(model.Email, model.Subject, model.Message);
                ViewBag.UserMessage = "Mail sent";

                //Clear all fields!
                ModelState.Clear();
            }
            else
            {
                //Show Errors
            }

            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            ViewBag.Title = "About Us";
            return View();
        }

        public IActionResult Shop()
        {
            var results = _context.Products.OrderBy(p => p.Category)
                                           .ToList();

            return View(results);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShakeotDay.API.Controllers;
using Microsoft.Extensions.Options;
using ShakeotDay.Core.Models;

namespace ShakeotDay.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Made for github game jam 11' 2016";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

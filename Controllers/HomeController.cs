using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.Models;

namespace webZone.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "Your application description page.";


            using(PsqlDal db = PsqlDal.Create()){
                var allprojects = db.projects.FirstOrDefault();
                Console.WriteLine(allprojects.ToString());
            }

            return View();
        }

        public IActionResult Youframe()
        {
            ViewData["Message"] = "Your frame!";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

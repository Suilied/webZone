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

        [HttpPost]
        public IActionResult CreateNewAccount(){
            // one-way encrypt PW
            // check if acc name exists in DB
            //  if it does; generate error
            // create new acc
            // generate success viewModel
            // return view
            return View();
        }

        //[AuthenticationMiddleware]
        public IActionResult Dashboard(){
            // check if logged in
            // if not; return error and redirect
            return View();
        }

        [HttpGet]
        public IActionResult Login(){
            // return login-form viewmodel
            return View();
        }

        [HttpPost]
        public IActionResult Login(string postData){
            // one-way encrypt PW
            // check if acc name exists in DB
            // check if PW matches
            // set user as logged in
            // return / redirect to dashboard

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

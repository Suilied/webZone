using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.Database.Models;
using webZone.Models;
using webZone.ViewModels;

namespace webZone.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NewAccount(){
            LoginViewModel viewModel = new LoginViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult NewAccount( LoginViewModel viewModel ){
            // one-way encrypt PW
            // check if acc name exists in DB
            //  if it does; generate error
            // create new acc
            // generate success viewModel
            // return view
            using (PsqlDal db = PsqlDal.Create()) {
                User existingUser = db.users.Where( x => x.username == viewModel.username ).FirstOrDefault();
                if (existingUser != null){
                    viewModel.loginError = "This username is already taken";
                    return View(viewModel);
                }

                db.users.Add( new User{
                    username = viewModel.username,
                    password = viewModel.password,
                    rememberMe = viewModel.rememberMe
                });

                db.SaveChanges();
            }

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
            LoginViewModel viewModel = new LoginViewModel();

            return View( viewModel );
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel){
            // one-way encrypt PW
            // check if acc name exists in DB
            // check if PW matches
            // set user as logged in
            // return / redirect to dashboard

            using (PsqlDal db = PsqlDal.Create())
            {
                User existingUser = db.users.Where( x => x.username == viewModel.username ).FirstOrDefault();
                if (existingUser == null){
                    viewModel.loginError = $"The user named: '{viewModel.username}' doesn't exist.";
                    return View(viewModel);
                }

                // TODO: fix the cookies and stuff
                // return the user to his or her dashboard
            }

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

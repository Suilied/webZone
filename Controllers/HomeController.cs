using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.Database.Models;
using webZone.Models;
using webZone.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
                    viewModel.errorMessage = "This username is already taken";
                    return View(viewModel);
                }

                Random rnd = new Random();
                Byte[] randomSalt = new byte[16];
                rnd.NextBytes(randomSalt);

                Byte[] hashedPassword = KeyDerivation.Pbkdf2(
                    password: viewModel.password,
                    salt: randomSalt,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 1000,
                    numBytesRequested: 32
                );
                string pwToSave = Convert.ToBase64String(hashedPassword);
                string saltToSave = Convert.ToBase64String(randomSalt);

                // wachtwoord = 
                // salt       = 


                db.users.Add( new User{
                    username = viewModel.username,
                    password = pwToSave,
                    salt = saltToSave,
                    rememberMe = viewModel.rememberMe
                });

                db.SaveChanges();
            }

            viewModel.username = "";
            viewModel.password = "";
            viewModel.rememberMe = false;

            return View(viewModel);
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
                    viewModel.errorMessage = $"The user named: '{viewModel.username}' doesn't exist.";
                    return View(viewModel);
                }

                // TODO: fix the cookies and stuff
                // return the user to his or her dashboard
            }

            viewModel.username = "";
            viewModel.password = "";
            viewModel.rememberMe = false;

            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

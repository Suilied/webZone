using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using webZone.Database;
using webZone.Database.Models;
using webZone.Models;
using webZone.ViewModels;
using webZone.Utilities;

namespace webZone.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string Test()
        {
            Hasher hasher = new Hasher("salt");

            string code64Salt = hasher.GetSalt();
            string code64Password = hasher.GetPassword("wachtwoord");

            if (User.Identity.IsAuthenticated)
                return "user authenticated + test complete!";

            return "test complete";
        }

        public string TestRandom()
        {
            Hasher hasher = new Hasher();

            string code64Salt = hasher.GetSalt();
            string code64Password = hasher.GetPassword("wachtwoord");

            return "test complete";
        }

        public IActionResult Dashboard( string account )
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            if (account == null )
                return RedirectToAction($"Dashboard?account={User.Identity.Name}");

            User existingUser = null;
            using (PsqlDal db = PsqlDal.Create())
            {
                existingUser = db.users.Where(x => x.username == account).FirstOrDefault();
            }

            // TODO: Dashboard info:
            // list of user info, name, email, etc.
            // list of user projects

            LoginViewModel viewModel = new LoginViewModel();
            viewModel.username = existingUser.username;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult NewAccount(){
            LoginViewModel viewModel = new LoginViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult NewAccount( LoginViewModel viewModel ){

            using (PsqlDal db = PsqlDal.Create()) {
                User existingUser = db.users.Where( x => x.username == viewModel.username ).FirstOrDefault();
                if (existingUser != null){
                    viewModel.errorMessage = "This username is already taken";
                    return View(viewModel);
                }

                Hasher hasher = new Hasher();
                
                db.users.Add( new User{
                    username = viewModel.username,
                    password = hasher.GetPassword( viewModel.password ),
                    salt = hasher.GetSalt(),
                    rememberMe = viewModel.rememberMe
                });

                db.SaveChanges();
            }

            viewModel.username = "";
            viewModel.password = "";
            viewModel.rememberMe = false;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Login(){
            // return login-form viewmodel
            LoginViewModel viewModel = new LoginViewModel();

            return View( viewModel );
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel){

            using (PsqlDal db = PsqlDal.Create())
            {
                User existingUser = db.users.Where( x => x.username == viewModel.username ).FirstOrDefault();
                if (existingUser == null){
                    viewModel.errorMessage = $"The user named: '{viewModel.username}' doesn't exist.";
                    return View(viewModel);
                }

                Hasher hasher = new Hasher(existingUser.salt);
                if (hasher.GetPassword(viewModel.password) != existingUser.password)
                {
                    viewModel.errorMessage = $"Invalid password!";
                    return View(viewModel);
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim( ClaimTypes.NameIdentifier, viewModel.username ));
                identity.AddClaim(new Claim( ClaimTypes.Name, viewModel.username ));
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = viewModel.rememberMe } );
            }

            viewModel.username = "";
            viewModel.password = "";
            viewModel.rememberMe = false;

            return View(viewModel);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync( CookieAuthenticationDefaults.AuthenticationScheme );
            return RedirectToAction("Login");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

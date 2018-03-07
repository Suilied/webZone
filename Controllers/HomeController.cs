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
using System.IO;
using System;

namespace webZone.Controllers
{
    public class HomeController : Controller
    {
        // TODO: delegate this to either config file or DB config table
        private static string _rootFolder = "wwwroot/projects";

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

            // it's called account because its a human readable string that relates to 
            // the chose account and is visible in the address bar
            // TODO: if we find a cleaner way to do this implement it.
            if (account == null )
                return RedirectToAction("Dashboard", new { account = User.Identity.Name });

            using (PsqlDal db = PsqlDal.Create())
            {
                User existingUser = db.users.Where(x => x.username == account).FirstOrDefault();

                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.username = existingUser.username;
                viewModel.projects = db.projects.Where(x => x.userId == existingUser.userId).ToList();

                return View(viewModel);
            }

        }

        [HttpPost]
        public IActionResult CreateNewProject([FromBody]Project project)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            User existingUser = null;
            using (PsqlDal db = PsqlDal.Create())
            {
                existingUser = db.users.Where(x => x.username == User.Identity.Name).FirstOrDefault();
            }

            Project newProject = new Project
            {
                userId = existingUser.userId,
                name = project.name,
                rootFolder = Guid.NewGuid().ToString()
            };

            int changesSaved = 0;

            using (PsqlDal db = PsqlDal.Create())
            {
                // TODO: clean the inputs
                var existingProject = db.projects.Where(x => x.name == newProject.name).FirstOrDefault();
                if (existingProject != null)
                    return Json(new { success = false, error = $"A project with the name: '{newProject.name}'; already exists." });

                db.projects.Add(newProject);
                changesSaved = db.SaveChanges();
            }

            // check if our write was successful
            if (changesSaved == 1)
            {
                // create the physical file and folder on disk
                string directoryPath = CrossPath.FixPath($"{Directory.GetCurrentDirectory()}/{_rootFolder}/{newProject.rootFolder}");
                string filePath = CrossPath.FixPath($"{Directory.GetCurrentDirectory()}/{_rootFolder}/{newProject.rootFolder}/main.js");

                System.IO.Directory.CreateDirectory(directoryPath);
                System.IO.File.Create(filePath);

                // don't forget to add the file to the database
                ProjectFile newProjectFile = new ProjectFile
                {
                    projectId = newProject.projectId,
                    name = "main.js",
                    type = "javascript"
                };

                using (PsqlDal db = PsqlDal.Create())
                {
                    // TODO: clean the inputs
                    db.projectFiles.Add(newProjectFile);
                    changesSaved = db.SaveChanges();
                }

                return Json(new { success = true, projectId = newProject.projectId });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        public JsonResult EditProject([FromBody]Project project)
        {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new { message = "User isn't logged in" });

            using (PsqlDal db = PsqlDal.Create())
            {
                Project existingProject = db.projects.Find(project.projectId);

                if (existingProject == null)
                    return new JsonResult(new { message = "Error, project doesn't exist." });

                existingProject.name = project.name;

                db.Entry(existingProject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
            }

            return new JsonResult(new { message = "Success" });
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
                    return View(new LoginViewModel());
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

            // Account creation succesfull, now lets log our user in
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, viewModel.username));
            identity.AddClaim(new Claim(ClaimTypes.Name, viewModel.username));
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = viewModel.rememberMe });

            return RedirectToAction("Dashboard", new { account = identity.Name });
        }

        [HttpGet]
        public IActionResult Login(){
            // return login-form viewmodel
            LoginViewModel viewModel = new LoginViewModel();

            return View( viewModel );
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel){

            bool success = false;

            using (PsqlDal db = PsqlDal.Create())
            {
                User existingUser = db.users.Where( x => x.username == viewModel.username ).FirstOrDefault();
                if (existingUser == null){
                    viewModel.errorMessage = $"The user named: '{viewModel.username}' doesn't exist.";
                    return View(new LoginViewModel());
                }

                Hasher hasher = new Hasher(existingUser.salt);
                if (hasher.GetPassword(viewModel.password) != existingUser.password)
                {
                    viewModel.errorMessage = $"Invalid password!";
                    return View(new LoginViewModel());
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim( ClaimTypes.NameIdentifier, viewModel.username ));
                identity.AddClaim(new Claim( ClaimTypes.Name, viewModel.username ));
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = viewModel.rememberMe } );

                success = true;
            }

            if( success)
            {
                return RedirectToAction("Dashboard", new { account = User.Identity.Name} );
            }

            return View(new LoginViewModel());
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

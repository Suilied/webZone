using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.Database.Models;
using webZone.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webZone.Controllers
{
    public class ImpulseController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ImpulseViewModel viewModel = new ImpulseViewModel();

            string projectName = "test";

            using (PsqlDal db = PsqlDal.Create())
            {
                viewModel.project = db.projects.Where(x => x.name == projectName).FirstOrDefault();
                viewModel.projectFiles = db.projectFiles.Where(x => x.projectId == viewModel.project.projectId).ToList();
            }

            return View();
        }
    }
}

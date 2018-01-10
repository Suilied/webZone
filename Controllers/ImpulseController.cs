using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.ViewModels;

namespace webZone.Controllers
{
    public class ImpulseController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index( string projectName )
        {
            if (projectName == null) {
                projectName = Global.Configuration["Impulse:DefaultProject"];
            }

            ImpulseViewModel viewModel = new ImpulseViewModel();

            using (PsqlDal db = PsqlDal.Create())
            {
                viewModel.project = db.projects.Where(x => x.name == projectName).FirstOrDefault();
                viewModel.projectFiles = db.projectFiles.Where(x => x.projectId == viewModel.project.projectId).ToList();
            }

            return View(viewModel);
        }

        public IActionResult Debug( string projectName )
        {
            if (projectName == null) {
                projectName = Global.Configuration["Impulse:DefaultProject"];
            }

            ImpulseViewModel viewModel = new ImpulseViewModel();

            using(PsqlDal db = PsqlDal.Create())
            {
                viewModel.project = db.projects.Where(x => x.name == projectName).FirstOrDefault();
                viewModel.projectFiles = db.projectFiles.Where(x => x.projectId == viewModel.project.projectId).ToList();
            }

            return View(viewModel);
        }
    }
}

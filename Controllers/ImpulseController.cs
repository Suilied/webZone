using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webZone.Database;
using webZone.ViewModels;

namespace webZone.Controllers
{
    public class ImpulseController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index( string projectId )
        {
            ImpulseViewModel viewModel = new ImpulseViewModel();

            using (PsqlDal db = PsqlDal.Create())
            {
                if (projectId == null)
                    viewModel.project = db.projects.Where(x => x.name == Global.Configuration["Impulse:DefaultProject"]).FirstOrDefault();
                else
                    viewModel.project = db.projects.Find(projectId);
                
                viewModel.projectFiles = db.projectFiles.Where(x => x.projectId == viewModel.project.projectId).ToList();
            }

            return View(viewModel);
        }

        public IActionResult Debug( string projectId )
        {
            ImpulseViewModel viewModel = new ImpulseViewModel();

            using(PsqlDal db = PsqlDal.Create())
            {
                if (projectId == null)
                    viewModel.project = db.projects.Where(x => x.name == Global.Configuration["Impulse:DefaultProject"]).FirstOrDefault();
                else
                    viewModel.project = db.projects.Find(projectId);
                
                viewModel.projectFiles = db.projectFiles.Where(x => x.projectId == viewModel.project.projectId).ToList();
            }

            return View(viewModel);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using webZone.ViewModels;
using System.Net;
using System.Text;
using webZone.Models;
using webZone.Database.Models;
using webZone.Database;
using webZone.Utilities;

namespace webZoneCore.Controllers
{
    public class RotideController : Controller
    {
        // TODO: delegate this to either config file or DB config table
        private static string _rootFolder = "wwwroot/projects";

        public IActionResult Index(string projectId)
        {
            RotideViewModel viewModel = new RotideViewModel();

            int projectInt;
            if( !int.TryParse(projectId, out projectInt) )
                return View(viewModel);

            using (PsqlDal db = PsqlDal.Create()) {
                viewModel.startProject = db.projects.Find(projectInt);
            }

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult GetFiles(string dir)
        {
            // dir isn't actually a directory but the name of a project.
            // when trying to get a subdirectory we must thusly first collect
            // the project name and then paste the rest on the end.
            dir = dir + "/";
            string projectName = dir.Split('/')[0];
            string subdirectories = dir.Replace(projectName, "");
            string projectRoot = "";

            using(PsqlDal db = PsqlDal.Create()) {
                string tempString = db.projects.Where( x => x.name == projectName).FirstOrDefault()?.rootFolder ?? "/";
                projectRoot = "/" + tempString;
            }

            string baseDir = Directory.GetCurrentDirectory();
            projectRoot = WebUtility.UrlDecode(projectRoot);
            
            string realDir = $"{baseDir}/{_rootFolder}{projectRoot}{subdirectories}";

            //validate to not go above basedir
            if (!realDir.StartsWith(baseDir))
            {
                realDir = baseDir;
                dir = "/";
            }

            List<FileTreeViewModel> files = new List<FileTreeViewModel>();

            DirectoryInfo di = new DirectoryInfo(realDir);

            foreach (DirectoryInfo dc in di.GetDirectories())
            {
                files.Add(new FileTreeViewModel() { Name = dc.Name, Path = String.Format("{0}{1}\\", dir, dc.Name), IsDirectory = true });
            }

            foreach (FileInfo fi in di.GetFiles())
            {
                files.Add(new FileTreeViewModel() { Name = fi.Name, Ext = fi.Extension.Substring(1).ToLower(), Path = dir + fi.Name, IsDirectory = false });
            }

            return PartialView("~/Views/Rotide/FileTree.cshtml", files);
        }

        public IActionResult GetFileContents(string project, string filepath)
        {
            // filepath isn't actually a directory but the name of a project.
            // when trying to get a subdirectory we must thusly first collect
            // the project name and then paste the rest on the end.
            project = project + "/";
            string projectName = project.Split('/')[0];
            string projectRoot = "";

            using (PsqlDal db = PsqlDal.Create())
            {
                projectRoot = db.projects.Where(x => x.name == projectName).FirstOrDefault()?.rootFolder ?? "/";
            }

            // string project isn't used anymore. But might get useful later on
            string filePath = CrossPath.FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{filepath.Replace(projectName, projectRoot)}" );

            var content = System.IO.File.ReadAllText(filePath);

            var retObj = new
            {
                path = filePath,
                content = content
            };

            return Json(retObj);
        }

        [HttpPost]
        public IActionResult SaveFileContents([FromBody]RotideFile file)
        {
            string filePath = CrossPath.FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{file.FilePath}" );

            try
            {
                System.IO.File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(file.FileContents));
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public IActionResult RebootInstance()
        {
            //Environment.Exit(1);

            RotideSettings settings = null;
            using (CoreDal db = CoreDal.Create())
            {
                settings = db.rotideSettings.First();
            }

            return Json(new { message = "rebuilding and restarting app..", rtSettings = settings.projectFolder });
        }

        public IActionResult Reboot()
        {
            return View("~/Views/Rotide/Reboot.cshtml");
        }

    }
}

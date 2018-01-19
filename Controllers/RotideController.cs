using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using webZone.ViewModels;
using System.Net;
using System.Text;
using webZone.Models;
using webZone.Database.Models;
using webZone.Database;

namespace webZoneCore.Controllers
{
    public class RotideController : Controller
    {
        private static string _rootFolder = "wwwroot/projects";

        public IActionResult Index(string project)
        {
            RotideViewModel viewModel = new RotideViewModel();

            using (PsqlDal db = PsqlDal.Create()) {
                viewModel.projects = db.projects.ToList();

                if (project != null)
                {
                    viewModel.startProject = db.projects.Where(x => x.name == project).FirstOrDefault();
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult GetFiles(string dir)
        {
            // dir isn't actually a directory but the name of a project.
            using(PsqlDal db = PsqlDal.Create()) {
                string tempString = db.projects.Where( x => x.name == dir).FirstOrDefault()?.rootFolder ?? "/";
                dir = "/" + tempString + "/";
            }

            string baseDir = Directory.GetCurrentDirectory();
            dir = WebUtility.UrlDecode(dir);
            
            string realDir = $"{baseDir}/{_rootFolder}{dir}";

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
            // string project isn't used anymore. But might get useful later on
            string filePath = FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{filepath}" );


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
            string filePath = FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{file.FilePath}" );

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
        public IActionResult CreateNewFolder([FromBody]RotideFile folderName)
        {
            string directoryPath = FixPath($"{Directory.GetCurrentDirectory()}/{_rootFolder}/{folderName}");

            System.IO.Directory.CreateDirectory(directoryPath);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CreateNewProject([FromBody]RotideNewProject aNewProject)
        {
            Project newProject = new Project
            {
                name = aNewProject.projectName,
                rootFolder = aNewProject.projectRoot
            };

            int changesSaved = 0;

            using (PsqlDal db = PsqlDal.Create())
            {
                // TODO: clean the inputs
                var existingProject = db.projects.Where(x => x.name == newProject.name).FirstOrDefault();
                if(existingProject != null)
                    return Json(new { success = false, error = $"A project with the name: '{newProject.name}'; already exists." });

                db.projects.Add(newProject);
                changesSaved = db.SaveChanges();
            }

            // check if our write was successful
            if( changesSaved == 1)
            {
                // create the physical file and folder on disk
                string directoryPath = FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{aNewProject.projectRoot}" );
                string filePath = FixPath( $"{Directory.GetCurrentDirectory()}/{_rootFolder}/{aNewProject.projectRoot}/main.js" );

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

        private string FixPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Fix filepaths for Ubuntu & Win
                path = path.Replace(@"/", @"\");
            }
            return path;
        }
    }
}

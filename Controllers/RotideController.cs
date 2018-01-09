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

        public IActionResult Index()
        {
            RotideViewModel viewModel = new RotideViewModel();

            using(PsqlDal db = PsqlDal.Create()) {
                viewModel.projects = db.projects.ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult GetFiles(string dir)
        {

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

        public IActionResult GetFileContents(string filepath)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/{_rootFolder}{filepath}";
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
            string filePath = $"{Directory.GetCurrentDirectory()}{file.FilePath}";
            filePath = filePath.Replace(@"/", @"\"); // this works on ubuntu, not sure about win and osx

            if( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) )
                return Json(new { success = false });

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

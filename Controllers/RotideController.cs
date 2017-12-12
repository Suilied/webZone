using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using webZoneCore.ViewModels;
using System.Net;
using System.Text;
using webZone.Models;

namespace webZoneCore.Controllers
{
    public class RotideController : Controller
    {
        public IActionResult Index()
        {
            // TODO: load document tree
            return View();
        }

        [HttpPost]
        public virtual ActionResult GetFiles(string dir)
        {

            //const string baseDir = @"/App_Data/userfiles/";
            string baseDir = Directory.GetCurrentDirectory();

            dir = WebUtility.UrlDecode(dir);
            string realDir = $"{baseDir}/{dir}"; //WebUtility.MapPath(baseDir + dir);

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

        public IActionResult GetAllFiles()
        {
            var someDir = new DirectoryInfo("./");
            var directories = someDir.GetDirectories();
            var files = someDir.GetFiles();

            List<string> dirItems = new List<string>();

            foreach (var dir in directories)
            {
                dirItems.Add(dir.Name);
            }

            foreach (var file in files)
            {
                dirItems.Add(file.Name);
            }

            var retObj = new
            {
                path = Directory.GetCurrentDirectory(),
                items = dirItems
            };

            return Json(retObj);
        }

        public IActionResult GetFileContents(string filepath)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}{filepath}";
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
            Environment.Exit(1);
            return Json(new { message = "rebuilding and restarting app.." });
        }

        public IActionResult Reboot()
        {
            return View("~/Views/Rotide/Reboot.cshtml");
        }
    }
}

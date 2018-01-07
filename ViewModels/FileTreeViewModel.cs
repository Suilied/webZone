using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webZone.ViewModels
{
    public class FileTreeViewModel
    {
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }

        public string PathAltSeparator()
        {
            return Path.Replace("\\", "/");
        }
    }
}

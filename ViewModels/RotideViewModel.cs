using System.Collections.Generic;
using webZone.Database.Models;

namespace webZone.ViewModels
{
    public class RotideViewModel
    {
        public List<Project> projects { get; set; }
        public Project startProject { get; set; }
    }
}

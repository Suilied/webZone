using System;
using System.Collections.Generic;
using webZone.Database.Models;

namespace webZone.ViewModels
{
    public class ImpulseViewModel
    {
        public Project project { get; set; }
        public List<ProjectFile> projectFiles { get; set; }
        //public bool debugMode { get; set; } = false;
    }
}

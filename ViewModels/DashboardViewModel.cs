using System.Collections.Generic;
using webZone.Database.Models;

namespace webZone.ViewModels
{
    public class DashboardViewModel
    {
        public string username { get; set; }
        public List<Project> projects { get; set; }
    }
}

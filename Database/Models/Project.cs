using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class Project
    {
        [Key]
        public int projectId { get; set; }

        public string projectName { get; set; }

        public string projectRootFolder { get; set; }
    }
}

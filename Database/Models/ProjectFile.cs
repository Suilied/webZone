using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class ProjectFile
    {
        [Key]
        public int projectFileId { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public int projectId { get; set; }
    }
}

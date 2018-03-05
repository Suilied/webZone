using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class Project
    {
        [Key]
        public int projectId { get; set; }

        public string name { get; set; }

        public string rootFolder { get; set; }

        public int userId { get; set; }
    }
}

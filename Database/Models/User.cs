using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public bool rememberMe { get; set; }
    }
}

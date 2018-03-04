using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        [MaxLength(32)]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string password { get; set; }

        [Required]
        [MaxLength(32)]
        public string salt { get; set; }

        public bool rememberMe { get; set; }
    }
}

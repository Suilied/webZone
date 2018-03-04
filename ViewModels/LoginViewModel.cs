using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string username { get; set; }

        [Required, DataType(DataType.Password)]
        public string password { get; set; }
        public bool rememberMe { get; set; }
        public string errorMessage { get; set; }
    }
}

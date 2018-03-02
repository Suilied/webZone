using System;
namespace webZone.ViewModels
{
    public class LoginViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool rememberMe { get; set; }
        public string loginError { get; set; }
    }
}

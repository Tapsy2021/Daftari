using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daftari.API.ViewModels
{
    public class LoginResult
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
    }
}
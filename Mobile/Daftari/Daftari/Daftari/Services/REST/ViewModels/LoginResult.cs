using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class LoginResult
    {
        //must return all valid data for user
        public string Message { get; set; }
        public string AccessToken { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
    }
}

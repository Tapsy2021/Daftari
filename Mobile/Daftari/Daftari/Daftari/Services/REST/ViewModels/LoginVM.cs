using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class LoginVM
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}

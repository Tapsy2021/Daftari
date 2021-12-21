using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class UserVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<ChildVM> Children { get; set; }
        public UserVM()
        {
            Children = new List<ChildVM>();
        }
    }
}

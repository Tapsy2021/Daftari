using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class UserViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<ChildViewModel> Children { get; set; }
        public UserViewModel()
        {
            Children = new List<ChildViewModel>();
        }
    }
}

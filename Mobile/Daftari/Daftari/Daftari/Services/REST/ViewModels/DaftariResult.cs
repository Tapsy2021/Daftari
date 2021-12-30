using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class DaftariResult
    {
        public object Body { get; set; }
        public List<string> ErrorMessages { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class ErrorResult
    {
        public string Message { get; set; }
    }
}

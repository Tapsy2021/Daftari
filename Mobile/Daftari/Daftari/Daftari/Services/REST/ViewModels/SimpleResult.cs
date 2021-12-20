using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Services.REST.ViewModels
{
    public class SimpleResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daftari.API.ViewModels
{
    public class DaftariResult<T>
    {
        public bool IsSuccess { get; set; }

        public List<string> ErrorMessages { get; set; }

        public T Body { get; set; }
    }
}
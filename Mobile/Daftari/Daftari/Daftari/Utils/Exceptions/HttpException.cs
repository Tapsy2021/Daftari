using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public class HttpException : Exception
    {
        public HttpException(string message) : base(message)
        {

        }
    }
}

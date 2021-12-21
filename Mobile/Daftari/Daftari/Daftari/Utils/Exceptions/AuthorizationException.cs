using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {

        }
    }
}

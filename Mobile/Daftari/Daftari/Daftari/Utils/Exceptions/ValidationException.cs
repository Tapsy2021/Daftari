﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}

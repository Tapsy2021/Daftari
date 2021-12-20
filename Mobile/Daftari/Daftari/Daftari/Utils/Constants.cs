using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public class Constants
    {
        public static class Expressions
        {
            public static readonly string Email_Regex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            public static readonly string Password_Regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
        }

        public static class Messages
        {
            public static readonly string Network_Error = "Oops, Something went wrong.";
            public static readonly string Network_Not_Available = "You have no internet access.";
        }
    }
}

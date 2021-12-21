using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Utils
{
    public class Constants
    {
        public static class URLs
        {
            public const string BaseURL = "http://muntumidev.com:8003/api/";

            public static readonly string Login = "account/login";
        }

        public static class Keys
        {
            public static readonly string Authorization_Code = "401 (Unauthorized)";
            public static readonly string Bearer = "Bearer";
            public static readonly string Device_Id = "DeviceId";
            public static readonly string Media_Type = "application/json";
            public static readonly string Is_First_Launch = "IsFirstLaunch";
        }

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

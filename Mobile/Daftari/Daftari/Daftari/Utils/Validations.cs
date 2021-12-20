using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Daftari.Utils
{
    public static class Validations
    {
        public static bool IsValidEmail(object sender, TextChangedEventArgs e)
        {
            var IsValid = (Regex.IsMatch(e.NewTextValue, Constants.Expressions.Email_Regex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).TextColor = IsValid ? Color.White : Color.Red;

            //(((Entry)sender).Parent.Parent as Frame).BorderColor = IsValid ? Color.FromHex("#bdbdbd") : Color.Red;
            return IsValid;
        }

        public static bool IsValidText(object sender, TextChangedEventArgs e)
        {
            var IsValid = !string.IsNullOrEmpty(e.NewTextValue);

            ((Entry)sender).TextColor = IsValid ? Color.White : Color.Red;
            //(((Entry)sender).Parent.Parent as Frame).BorderColor = IsValid ? Color.FromHex("#bdbdbd") : Color.Red;
            return IsValid;
        }

        public static bool IsValidCellphone(object sender, TextChangedEventArgs e)
        {
            bool IsValid = double.TryParse(e.NewTextValue, out double result) && e.NewTextValue.Trim().Length >= 9;

            ((Entry)sender).TextColor = IsValid ? Color.White : Color.Red;
            //(((Entry)sender).Parent.Parent as Frame).BorderColor = IsValid ? Color.FromHex("#bdbdbd") : Color.Red;
            return IsValid;
        }
        public static bool IsValidPassword(object sender, TextChangedEventArgs e)
        {
            var IsValid = (Regex.IsMatch(e.NewTextValue, Constants.Expressions.Password_Regex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).TextColor = IsValid ? Color.White : Color.Red;

            //(((Entry)sender).Parent.Parent as Frame).BorderColor = IsValid ? Color.FromHex("#bdbdbd") : Color.Red;
            return IsValid;
        }

        public static bool IsPasswordMatch(object sender, Entry Password)
        {
            var IsValid = (((Entry)sender).Text ?? "").Trim().Equals(Password.Text?.Trim());
            ((Entry)sender).TextColor = IsValid ? Color.White : Color.Red;

            //(((Entry)sender).Parent.Parent as Frame).BorderColor = IsValid ? Color.FromHex("#bdbdbd") : Color.Red;
            return IsValid;
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet || current == NetworkAccess.Unknown)
                {
                    return true;
                    // Connection to internet is available
                }
                return false;
            }
            catch { }
            return true;
        }
    }
}

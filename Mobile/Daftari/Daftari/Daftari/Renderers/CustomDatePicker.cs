using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Daftari.Renderers
{
    public class CustomDatePicker : DatePicker
    {
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomDatePicker));
        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(string), typeof(CustomDatePicker));

        public string Placeholder
        {
            get { return (string)this.GetValue(PlaceholderProperty); }
            set { this.SetValue(PlaceholderProperty, value); }
        }

        public string PlaceholderColor
        {
            get { return (string)this.GetValue(PlaceholderColorProperty); }
            set { this.SetValue(PlaceholderColorProperty, value); }
        }
    }
}

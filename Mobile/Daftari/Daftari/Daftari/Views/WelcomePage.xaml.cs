using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void OnLogin_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Auth.LoginPage());
        }

        private void OnSignUp_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Auth.SignUpPage());
        }
    }
}
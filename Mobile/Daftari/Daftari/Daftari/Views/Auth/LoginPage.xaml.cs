using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void OnSignUp_Clicked(object sender, EventArgs e)
        {
            //Navigation.InsertPageBefore(new MasterHomePage(), this);
            //await Navigation.PopAsync();
        }

        private async void OnLogIn_Clicked(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new MasterHomePage(), this);
            await Navigation.PopAsync();
        }
    }
}
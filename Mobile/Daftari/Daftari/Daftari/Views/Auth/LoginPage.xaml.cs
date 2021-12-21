using Daftari.Models;
using Daftari.Services.Database;
using Daftari.Services.Depedencies;
using Daftari.Services.REST.Helpers;
using Daftari.Services.REST.ViewModels;
using Daftari.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private CancellationTokenSource _ct_Token;
        public bool IsRunning { get; set; }
        public LoginPage()
        {
            InitializeComponent();
            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);

            _ct_Token = new CancellationTokenSource();
            BindingContext = this;
        }

        private void OnSignUp_Clicked(object sender, EventArgs e)
        {
            //Navigation.InsertPageBefore(new MasterHomePage(), this);
            //await Navigation.PopAsync();
        }

        private async void OnLogIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!Validations.IsValidText(Username, new TextChangedEventArgs(null, Username.Text ?? "")))
                {
                    Username_Error.Text = "Username is required*";
                    Username_Error.IsVisible = true;
                    //Toast?
                    Username.Focus();
                    return;
                }
                Username_Error.IsVisible = false;
                Username_Error.Text = "";

                if (!Validations.IsValidText(Password, new TextChangedEventArgs(null, Password.Text ?? "")))
                {
                    Username_Error.Text = "Password is required*";
                    Username_Error.IsVisible = true;
                    //Toast?
                    Username.Focus();
                    return;
                }
                Password_Error.IsVisible = false;
                Password_Error.Text = "";

                if (!_ct_Token.IsCancellationRequested)
                    _ct_Token.Cancel();
                _ct_Token = new CancellationTokenSource();

                IsRunning = true;
                OnPropertyChanged("IsRunning");

                var response = await AuthHelper.Login(new LoginVM
                {
                    Password = Password.Text.Trim(),
                    UserName = Username.Text.Trim(),
                    //DeviceId = FCMToken, //(Application.Current as App).DeviceId,
                    DeviceName = DeviceInfo.Name,
                    //FCMToken = FCMToken
                }, _ct_Token);
                IsRunning = false;
                OnPropertyChanged("IsRunning");
                if (!response.IsSuccess)
                {
                    DependencyService.Get<IMessage>().LongAlert(response.Message);

                    Password.Text = "";
                    Username.TextColor = Color.Red;

                    Username_Error.Text = "Invalid Username or Password";
                    Username_Error.IsVisible = true;

                    Username.Focus();
                    return;
                }

                var result = JsonConvert.DeserializeObject<LoginResult>(response.Body.ToString());

                var user = new User
                {
                    // put other fields
                    Token = result.AccessToken,
                    FirstName = result.FirstName,
                    LastName = result.LastName
                };

                await DbHelper.Instance.SaveUser(user);
                (Application.Current as App).Identity = user;

                Navigation.InsertPageBefore(new MasterHomePage(), this);
                await Navigation.PopAsync();

            }
            catch { }
        }

        void OnUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var parent = ((Entry)sender).Parent as StackLayout;
                var error = (Label)parent.Children[1];

                if (Validations.IsValidText(sender, e))
                {
                    error.IsVisible = false;
                    error.Text = "";
                }
            }
            catch { }
        }

        void OnPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var parent = ((Entry)sender).Parent as StackLayout;
                var error = (Label)parent.Children[1];

                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    error.IsVisible = false;
                    error.Text = "";
                }
            }
            catch { }
        }

        
    }
}
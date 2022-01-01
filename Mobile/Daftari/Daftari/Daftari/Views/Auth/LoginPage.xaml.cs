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
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

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
                    Password_Error.Text = "Password is required*";
                    Password_Error.IsVisible = true;
                    //Toast?
                    Password.Focus();
                    return;
                }
                Password_Error.IsVisible = false;
                Password_Error.Text = "";

                if (!_ct_Token.IsCancellationRequested)
                    _ct_Token.Cancel();
                _ct_Token = new CancellationTokenSource();

                //var user = new User
                //{
                //    // put other fields
                //    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJhMjI3ZDFiOS1mMmNkLTQyMDUtYjgyNS1jMmMzMmM4OTMzOWYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoib3VwYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3NpZCI6IjkxMzRhMWUyLTQxYTUtNGVmNC1iMmUzLWRiNWE4YzIxOTJlMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE2NDI2ODQ3NTgsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzY3IiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNjcifQ.n7SPpYsYftXl2YP0UBX - QsI620DT66QqhXlkqhwxuuk",
                //    FirstName = "Oupa",
                //    LastName = "Mokone"
                //};

                IsRunning = true;
                //OnPropertyChanged("IsRunning");

                var response = await AuthHelper.Login(new LoginVM
                {
                    Password = Password.Text.Trim(),
                    UserName = Username.Text.Trim(),
                    //DeviceId = FCMToken, //(Application.Current as App).DeviceId,
                    DeviceName = DeviceInfo.Name,
                    //FCMToken = FCMToken
                }, _ct_Token);
                IsRunning = false;
                //OnPropertyChanged("IsRunning");
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
                    LastName = result.LastName,
                    Photo = result.Photo
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

                if (Validations.IsValidText(sender, e))
                {
                    error.IsVisible = false;
                    error.Text = "";
                }
            }
            catch { }
        }

        
    }
}
using Daftari.Services.REST.ViewModels;
using Daftari.Utils;
using Daftari.ViewModels;
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
    public partial class SignUpPage : TabbedPage
    {
        private UserVM user { get; set; }
        public SignUpPage()
        {
            InitializeComponent();

            user = new UserVM();
            user.Children.Add(new ChildVM());
            DOB.MaximumDate = DateTime.Today.AddDays(-1);
            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);
            // This disables swiping in android as it is not needed
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);

            BindingContext = new SignUpViewModel();
        }

        void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var parent = ((Entry)sender).Parent as StackLayout;

                var error = (Label)parent.Children[1];

                if (Validations.IsValidText(sender, e))
                {
                    error.IsVisible = false;
                    error.Text = "";
                    //error.Text = "This field is required*";
                }

            } catch { }
        }

        void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var parent = ((Entry)sender).Parent as StackLayout;

                var error = (Label)parent.Children[1];

                if (Validations.IsValidEmail(sender, e))
                {
                    error.IsVisible = false;
                    error.Text = "";
                    //error.Text = "This field is required*";
                }

            }
            catch { }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validations.IsValidPassword(sender, e))
            {
                Password_Error.Text = "Password should include a Capital letter, Special Character and a number.";
                Password_Error.IsVisible = true;
            }
            else
            {
                Password_Error.IsVisible = false;
                Password_Error.Text = "";                
            }
        }

        private void ConfirmPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validations.IsPasswordMatch(sender, Password))
            {
                ConfirmPassword_Error.Text = "Confirmation does not match password";
                ConfirmPassword_Error.IsVisible = true;
            }
            else
            {
                ConfirmPassword_Error.IsVisible = false;
                ConfirmPassword_Error.Text = "";
            }
        }

        private void Gender_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gender = Gender.SelectedItem as string;
            user.Children[0].Gender = gender;
            try
            {
                var parent = Gender.Parent as StackLayout;
                var error = (Label)parent.Children[1];

                error.IsVisible = false;
                error.Text = "";
            }
            catch { }
        }

        #region Navigation
        void NextTab()
        {
            var Index = Children.IndexOf(CurrentPage);
            if (Index < Children.Count - 1)
            {
                CurrentPage = Children[Index + 1];
            }
        }

        private void OnBackButton_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {
            var Index = Children.IndexOf(CurrentPage);
            if (Index > 0)
            {
                CurrentPage = Children[Index - 1];
                return true;
            }

            return base.OnBackButtonPressed();
        }
        #endregion

        private void OnCredentials_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!Validations.IsValidText(FullName, new TextChangedEventArgs(null, FullName.Text ?? "")))
                {
                    try
                    {
                        var parent = FullName.Parent as StackLayout;
                        var error = (Label)parent.Children[1];

                        error.Text = "Full Name is required*";
                        error.IsVisible = true;
                    }
                    catch { }
                    //Toast?
                    FullName.Focus();
                    return;
                }
                if (!Validations.IsValidEmail(Email, new TextChangedEventArgs(null, Email.Text ?? "")))
                {
                    try
                    {
                        var parent = Email.Parent as StackLayout;
                        var error = (Label)parent.Children[1];

                        error.Text = "Invalid Email*";
                        error.IsVisible = true;
                    }
                    catch { }
                    //Toast?
                    Email.Focus();
                    return;
                }

                if (!Validations.IsValidPassword(Password, new TextChangedEventArgs(null, Password.Text ?? "")))
                {
                    Password_Error.Text = "Password should include a Capital letter, Special Character and a number.";
                    Password.Focus();
                    return;
                }
                Password_Error.Text = "";

                if (!Validations.IsPasswordMatch(ConfirmPassword, Password))
                {
                    ConfirmPassword_Error.Text = "Confirmation does not match password";
                    ConfirmPassword.Focus();
                    return;
                }

                ConfirmPassword_Error.Text = "";

                user.FullName = FullName.Text.Trim();
                user.Email = Email.Text;
                user.Password = Password.Text.Trim();
            }
            catch { }
            NextTab();
        }

        private void OnValidateChild_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!Validations.IsValidText(Name, new TextChangedEventArgs(null, Name.Text ?? "")))
                {
                    try
                    {
                        var parent = Name.Parent as StackLayout;
                        var error = (Label)parent.Children[1];

                        error.Text = "Name is required*";
                        error.IsVisible = true;
                    }
                    catch { }
                    //Toast?
                    Name.Focus();
                    return;
                }

                if (DOB.SelectedItem == null)
                {
                    try
                    {
                        var parent = DOB.Parent as StackLayout;
                        var error = (Label)parent.Children[1];

                        error.Text = "Date of birth is required*";
                        error.IsVisible = true;
                    }
                    catch { }
                    //DOB.Focus();
                    return;
                }

                if (Gender.SelectedItem == null)
                {
                    try
                    {
                        var parent = Gender.Parent as StackLayout;
                        var error = (Label)parent.Children[1];

                        error.Text = "Gender is required*";
                        error.IsVisible = true;
                    }
                    catch { }
                    Gender.Focus();
                    return;
                }
            } catch { }

            user.Children[0].FullName = Name.Text.Trim();
            user.Children[0].DateOfBirth = DOB.Date;
            user.Children[0].Gender = Gender.SelectedItem as string;
            NextTab();
        }

        //private void OnSwimLevel_Clicked(object sender, EventArgs e)
        //{
        //    NextTab();
        //}

        private void DOB_DateSelected(object sender, DateChangedEventArgs e)
        {
            var date = DOB.Date;
            user.Children[0].DateOfBirth = date;
            try
            {
                var parent = DOB.Parent as StackLayout;
                var error = (Label)parent.Children[1];

                error.IsVisible = false;
                error.Text = "";
            }
            catch { }
        }
    }
}
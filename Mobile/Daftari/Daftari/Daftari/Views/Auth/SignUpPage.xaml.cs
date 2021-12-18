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
        public SignUpPage()
        {
            InitializeComponent();

            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);
            // This disables swiping in android as it is not needed
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);

            BindingContext = new SignUpViewModel();
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
            NextTab();
        }

        private void OnValidateChild_Clicked(object sender, EventArgs e)
        {
            NextTab();
        }

        private void OnSwimLevel_Clicked(object sender, EventArgs e)
        {
            NextTab();
        }
    }
}
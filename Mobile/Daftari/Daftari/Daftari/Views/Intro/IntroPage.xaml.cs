using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views.Intro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntroPage : TabbedPage
    {
        public IntroPage()
        {
            InitializeComponent();
            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);
            // This disables swiping in android as it is not needed
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
        }

        private void NextPage_Clicked(object sender, EventArgs e)
        {
            NextTab();
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

        private void OnLogin_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new MasterHomePage());
        }

        private void OnSignUp_Clicked(object sender, EventArgs e)
        {

        }
    }
}
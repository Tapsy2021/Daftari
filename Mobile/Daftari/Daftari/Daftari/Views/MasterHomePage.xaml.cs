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
    public partial class MasterHomePage : MasterDetailPage
    {
        public MasterHomePage()
        {
            InitializeComponent();
            // This hides the navigation page's navigation bar as it is not needed
            NavigationPage.SetHasNavigationBar(this, false);
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as HomeMenuItem;
            if (item == null)
                return;

            try
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);                
                if (page.GetType() == typeof(AquaCard.CardsPage))
                {
                    Navigation.PushAsync(page, true);
                }
                else
                {
                    page.Title = item.Title;
                    Detail = new NavigationPage(page);
                }
                IsPresented = false;

                MasterPage.ListView.SelectedItem = null;
            } catch { }
        }
    }
}
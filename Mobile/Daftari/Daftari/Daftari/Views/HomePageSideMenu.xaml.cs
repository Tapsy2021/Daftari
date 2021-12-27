using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageSideMenu : ContentPage
    {
        public ListView ListView;

        public HomePageSideMenu()
        {
            InitializeComponent();

            BindingContext = new HomePageSideMenuVM();
            ListView = MenuItemsListView;
        }

        class HomePageSideMenuVM : INotifyPropertyChanged
        {
            public string FullName
            {
                get
                {
                    return $"{(Application.Current as App).Identity.FirstName}'s Daftari";
                }
            }
            public ObservableCollection<HomeMenuItem> MenuItems { get; set; }

            public HomePageSideMenuVM()
            {
                MenuItems = new ObservableCollection<HomeMenuItem>(new[]
                {
                    new HomeMenuItem { Id = 0, Title = "Home",Icon = "icon_home.png", TargetType = typeof(Home.HomePageDetail) },
                    new HomeMenuItem { Id = 1, Title = "Swim Finder", Icon = "icon_swim_finder.png" },
                    new HomeMenuItem { Id = 2, Title = "Price List", Icon = "icon_checklist.png" },
                    new HomeMenuItem { Id = 3, Title = "Aqua Card", Icon = "icon_aqua_cards.png", TargetType = typeof(AquaCard.CardsPage) },
                    new HomeMenuItem { Id = 4, Title = "Team Oman", Icon= "icon_people.png" },
                    new HomeMenuItem { Id = 5, Title = "Shop Now", Icon= "icon_people.png" },
                    new HomeMenuItem { Id = 6, Title = "Contact Us", Icon= "icon_people.png", TargetType = typeof(Communication.ContactUsPage) },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        private void OnLogOut_Tapped(object sender, EventArgs e)
        {
            Services.Database.DbHelper.Instance.SignOut();
            Application.Current.MainPage = new NavigationPage(new Auth.LoginPage());
        }
    }
}